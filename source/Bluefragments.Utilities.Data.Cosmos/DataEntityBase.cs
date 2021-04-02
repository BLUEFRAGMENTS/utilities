using Newtonsoft.Json;

namespace Bluefragments.Utilities.Data.Cosmos
{
    public abstract class DataEntityBase<T> : ICosmosEntityBase<T>
    {
        [JsonProperty("id")]
        public T Id { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}
