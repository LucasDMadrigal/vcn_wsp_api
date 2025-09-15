using System.Text.Json.Serialization;

namespace Chat.Shared.ApiMetaDTOs
{
    public class ConversationDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("origin")]
        public OriginDto Origin { get; set; }
    }
}