using System.Text.Json.Serialization;

namespace Chat.Shared.ApiMetaDTOs
{
    public class MetadataDto
    {
        [JsonPropertyName("display_phone_number")]
        public string DisplayPhoneNumber { get; set; }

        [JsonPropertyName("phone_number_id")]
        public string PhoneNumberId { get; set; }
    }
}