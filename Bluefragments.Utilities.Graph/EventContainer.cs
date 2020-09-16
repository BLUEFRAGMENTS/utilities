using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Bluefragments.Utilities.Graph
{
    public class EventContainer
    {
        [JsonProperty(PropertyName = "value")]
        public Event[] Value { get; set; }
    }
}
