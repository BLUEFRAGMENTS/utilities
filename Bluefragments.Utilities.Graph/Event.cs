using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Bluefragments.Utilities.Graph
{
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
}
