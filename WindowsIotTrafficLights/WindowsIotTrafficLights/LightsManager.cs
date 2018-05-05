using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsIotTrafficLights
{
    public class LightsManager
    {
        public delegate void ScheduleChangedEventHandler(string newScheduleId);
        public event ScheduleChangedEventHandler ScheduleChanged;

        private readonly IEnumerable<ISwitcher> _switchers;
        private readonly IScheduleUpdateClient _updateClient;

        private Timer _scheduleUpdateTimer;
        private Task _schedulePlayerTask;
        private Schedule _schedule;
        private bool _updating;

        public LightsManager(ISwitcher switcher, IScheduleUpdateClient updateClient) : 
            this(new []{ switcher }, updateClient)
        {
        }

        public LightsManager(IEnumerable<ISwitcher> switchers, IScheduleUpdateClient updateClient)
        {
            _switchers = switchers;
            _updateClient = updateClient;
        }

        public void Start()
        {
            _schedulePlayerTask = GetSchedulePlayer();
            _scheduleUpdateTimer = new Timer(OnUpdateTimer, null, 0, 60000);
        }

        private Task GetSchedulePlayer()
        {
            return Task.Run(async () =>
            {
                while (true)
                {
                    if (_updating)
                    {
                        Debug.WriteLine("Schedule update, closing player");
                        return;
                    }

                    if (!CanWorkOnSchedule())
                    {
                        await Task.Delay(5000);
                        continue;
                    }

                    foreach (var item in _schedule.Items)
                    {
                        foreach (var switcher in _switchers)
                        {
                            if(switcher == null)
                            {
                                continue;
                            }

                            switcher.SwitchToItem(item);
                        }

                        await Task.Delay(item.Interval);
                    }
                }
            });
        }

        private void OnUpdateTimer(object state)
        {
            var newSchedule = _updateClient.UpdateSchedule();
            if (_schedule != null && newSchedule.Id == _schedule.Id)
            {
                Debug.WriteLine("Schedule doesn't need update");
                return;
            }

            Debug.WriteLine("Updating schedule to " + newSchedule.Id);
            _updating = true;
            _schedulePlayerTask.Wait();
            _schedulePlayerTask.Dispose();

            Debug.WriteLine("Assigning new schedule");
            _schedule = newSchedule;
            _updating = false;

            Debug.WriteLine("Firing schedule changed event");
            ScheduleChanged?.Invoke(_schedule.Id);

            Debug.WriteLine("Starting schedule player");
            _schedulePlayerTask = GetSchedulePlayer();
        }

        private bool CanWorkOnSchedule()
        {
            if (_schedule == null)
            {
                Debug.WriteLine("Schedule is null, waiting ...");
                return false;
            }

            if (_schedule.Items == null)
            {
                Debug.WriteLine("Schedule items collection is null ...");
                return false;
            }

            if (_schedule.Items.Count == 0)
            {
                Debug.WriteLine("Schedule has no items ...");
                return false;
            }

            return true;
        }
    }
}
