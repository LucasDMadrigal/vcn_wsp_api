namespace Chat.Services.Services
{
    public interface IMetaService
    {
        Task<(string? metaMessageId, int statusCode, string responseBody)> SendTemplateAsync(
        string to, string templateName, string languageCode);
    }
}
