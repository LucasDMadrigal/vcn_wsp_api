
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Chat.Services.Services.ServiceImpl
{
    public class MetaService : IMetaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        public async Task<(string? metaMessageId, int statusCode, string responseBody)> SendTemplateAsync(string to, string templateName, string languageCode)
        {
            var client = _httpClientFactory.CreateClient();
            var token = _config["Meta:AccessToken"];
            var phoneNumberId = _config["Meta:PhoneNumberId"];
            var url = $"https://graph.facebook.com/v22.0/{phoneNumberId}/messages";

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to,
                type = "template",
                template = new
                {
                    name = templateName,
                    language = new { code = languageCode }
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            var body = await response.Content.ReadAsStringAsync();

            string? metaMessageId = null;
            try
            {
                using var doc = JsonDocument.Parse(body);
                metaMessageId = doc.RootElement.GetProperty("messages")[0].GetProperty("id").GetString();
            }
            catch { /* ignore parse errors */ }

            return (metaMessageId, (int)response.StatusCode, body);
        }
    }
}
