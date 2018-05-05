using System.Net;
using System.Net.Cache;
using Newtonsoft.Json;

namespace WindowsIotTrafficLights
{
    public class ScheduleUpdateClient
    {
        private readonly string _updateUrl;

        public ScheduleUpdateClient(string updateUrl)
        {
            _updateUrl = updateUrl;
        }

        public Schedule UpdateSchedule()
        {
            using (var client = new WebClient())
            {
                client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);

                var json = client.DownloadString(_updateUrl);

                return JsonConvert.DeserializeObject<Schedule>(json);
            }
        }
    }
}
