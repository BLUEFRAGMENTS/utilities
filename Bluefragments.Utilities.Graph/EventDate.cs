using System;
using Newtonsoft.Json;

namespace Bluefragments.Utilities.Graph
{
    public class EventDate
    {
        [JsonProperty(PropertyName = "dateTime")]
        public DateTime DateTime { get; set; }

        [JsonProperty(PropertyName = "timeZone")]
        public string TimeZone { get; set; }
    }
}