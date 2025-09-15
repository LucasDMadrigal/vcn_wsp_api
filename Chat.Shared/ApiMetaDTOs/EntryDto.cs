using Chat.Shared.ApiMetaDTOs;
using System.Text.Json.Serialization;

namespace Chat.Shared.ApiMetaDTOs
{
    public class EntryDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("changes")]
        public List<ChangeDto> Changes { get; set; }
    }
}