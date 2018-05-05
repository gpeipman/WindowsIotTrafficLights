using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;
using WindowsIotTrafficLights;

namespace WindowsIotTrafficLightsService
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        private Schedule _schedule = new Schedule();
        private ScheduleUpdateClient _updateClient;

        private bool _updating;
        private Timer _scheduleUpdateTimer;
        private Task _schedulePlayerTask;

        private GpioPin _redPin;
        private GpioPin _yellowPin;
        private GpioPin _greenPin;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            InitGpioPins();

            _updateClient = new ScheduleUpdateClient("http://gunnarpeipman.com/trafficlightsupdate.php");
            _scheduleUpdateTimer = new Timer(OnUpdateTimer, null, 0, 60000);
            _schedulePlayerTask = GetSchedulePlayer();
        }

        private Task GetSchedulePlayer()
        {
            return Task.Run(async () =>
            {
                while (true)
                {
                    if(_updating)
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
                        _redPin.Write(item.Red ? GpioPinValue.High : GpioPinValue.Low);
                        _yellowPin.Write(item.Yellow ? GpioPinValue.High : GpioPinValue.Low);
                        _greenPin.Write(item.Green ? GpioPinValue.High : GpioPinValue.Low);

                        await Task.Delay(item.Interval);
                    }
                }
            });
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

        private void OnUpdateTimer(object state)
        {
            var newSchedule = _updateClient.UpdateSchedule();
            if(newSchedule.Id == _schedule.Id)
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

            Debug.WriteLine("Starting schedule player");
            _schedulePlayerTask = GetSchedulePlayer();
        }

        private void InitGpioPins()
        {
            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                Debug.WriteLine("GPIO controller is missing!");
                return;
            }

            _redPin = gpio.OpenPin(17);
            _yellowPin = gpio.OpenPin(18);
            _greenPin = gpio.OpenPin(27);

            _redPin.Write(GpioPinValue.Low);
            _redPin.SetDriveMode(GpioPinDriveMode.Output);

            _yellowPin.Write(GpioPinValue.Low);
            _yellowPin.SetDriveMode(GpioPinDriveMode.Output);

            _greenPin.Write(GpioPinValue.Low);
            _greenPin.SetDriveMode(GpioPinDriveMode.Output);
        }
    }
}
