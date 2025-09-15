using Chat.Shared.ApiMetaDTOs;
using System.Text.Json.Serialization;

namespace Chat.Shared.ApiMetaDTOs
{
    public class MessageSentMetaResponseDto
    {
        [JsonPropertyName("messaging_product")]
        public string MessagingProduct { get; set; }

        [JsonPropertyName("contacts")]
        public List<ContactDto> Contacts { get; set; }

        [JsonPropertyName("messages")]
        public List<SentMessageDto> Messages { get; set; }
    }
}
