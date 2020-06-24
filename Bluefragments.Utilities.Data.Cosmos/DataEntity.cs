using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Bluefragments.Utilities.Data.Cosmos
{
    public abstract class DataEntity : ICosmosEntity
    {
        [JsonProperty(PropertyName = "id")]
        public object Id { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}
