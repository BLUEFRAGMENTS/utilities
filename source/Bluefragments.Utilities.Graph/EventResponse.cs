using Newtonsoft.Json;

namespace Bluefragments.Utilities.Graph
{
    public class EventResponse
    {
        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }

        [JsonProperty(PropertyName = "sendResponse")]
        public bool Response { get; set; }
    }
}
