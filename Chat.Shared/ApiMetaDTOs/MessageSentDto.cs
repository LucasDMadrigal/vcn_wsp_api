using Chat.Shared.ApiMetaDTOs;

namespace Chat.Shared.ApiMetaDTOs
{
    public class MessageSentDto
    {
        public string MessagingProduct { get; set; } = "whatsapp"; // siempre igual
        public string RecipientType { get; set; } = "individual"; // opcional (para text)
        public string To { get; set; }
        public string Type { get; set; }

        public TextMessageDto? Text { get; set; }
        public TemplateMessageDto? Template { get; set; }
    }
}
