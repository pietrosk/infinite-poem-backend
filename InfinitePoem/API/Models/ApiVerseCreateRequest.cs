using Newtonsoft.Json;

namespace API.Models
{
    public class ApiVerseCreateRequest
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }
}
