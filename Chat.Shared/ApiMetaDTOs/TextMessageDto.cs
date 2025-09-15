namespace Chat.Shared.ApiMetaDTOs
{
    public class TextMessageDto
    {
        public bool PreviewUrl { get; set; } = false;
        public string Body { get; set; }
    }
}