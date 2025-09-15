using System.Text.Json.Serialization;

namespace Chat.Shared.ApiMetaDTOs
{
    public class SentMessageDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("message_status")]
        public string MessageStatus { get; set; }
    }
}