namespace WindowsIotTrafficLights
{
    public class EventBasedSwitcher : ISwitcher
    {
        public delegate void ItemSwicthedEventHandler(ScheduleItem item);
        public event ItemSwicthedEventHandler ItemSwitched;

        public void SwitchToItem(ScheduleItem item)
        {
            ItemSwitched?.Invoke(item);
        }
    }
}
