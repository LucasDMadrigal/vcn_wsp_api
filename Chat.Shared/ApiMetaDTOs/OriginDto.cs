using System.Text.Json.Serialization;

namespace Chat.Shared.ApiMetaDTOs
{
    public class OriginDto
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}