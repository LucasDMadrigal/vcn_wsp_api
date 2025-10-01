namespace Chat.Shared.DTOs
{
    public class ConversationResponseDto
    {
        public string Id { get; set; }
        public string WaId { get; set; }
        public string ClientId { get; set; }
        public bool IsOpen { get; set; }
        public List<MessageDto> Messages { get; set; }
    }
}
