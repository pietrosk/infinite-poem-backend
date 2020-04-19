using Newtonsoft.Json;

namespace InfinitePoem.DAO
{
    public class Verse
    {
        [JsonProperty(PropertyName = "_pk")]
        public string PartitionKey { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public string CreatedAt { get; set; }
    }
}
