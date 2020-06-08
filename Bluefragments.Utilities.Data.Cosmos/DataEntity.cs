using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Bluefragments.Utilities.Data.Cosmos
{
    public abstract class DataEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}
