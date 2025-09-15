using System.Text.Json.Serialization;

namespace Chat.Shared.ApiMetaDTOs
{
    public class MessageSendWebhookReceivedDto
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("entry")]
        public List<EntryDto> Entry { get; set; }
    }
}
