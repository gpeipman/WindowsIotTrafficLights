using System.Collections.Generic;

namespace WindowsIotTrafficLights
{
    public class Schedule
    {
        public string Id;

        public IList<ScheduleItem> Items = new List<ScheduleItem>();
    }
}
