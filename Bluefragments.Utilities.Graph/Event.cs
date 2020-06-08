using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bluefragments.Utilities.Graph
{
    public class EventContainer
    {
        [JsonProperty(PropertyName = "value")]
        public Event[] Value { get; set; }
    }

    public class Event
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }
        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }
        [JsonProperty(PropertyName = "start")]
        public EventDate Start { get; set; }
        [JsonProperty(PropertyName = "end")]
        public EventDate End { get; set; }
        [JsonProperty(PropertyName = "isOrganizer")]
        public bool IsOrganizer { get; set; }
    }

    public class EventDate
    {
        [JsonProperty(PropertyName = "dateTime")]
        public DateTime DateTime { get; set; }
        [JsonProperty(PropertyName = "timeZone")]
        public string TimeZone { get; set; }
    }

    public class EventResponse
    {
        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }
        [JsonProperty(PropertyName = "sendResponse")]
        public bool Response { get; set; }
    }
}
