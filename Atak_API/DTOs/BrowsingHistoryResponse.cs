using System.Text.Json.Serialization;

namespace Atak_API.DTOs
{
    public class BrowsingHistoryResponse 
    {
        [JsonPropertyName("user-id")]
        public string UserId { get; set; }

        [JsonPropertyName("products")]
        public List<string> Products { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
