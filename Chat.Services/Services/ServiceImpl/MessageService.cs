using Chat.Domain.Entities;
using Chat.Domain.Interfaces;

namespace Chat.Services.Services.ServiceImpl
{
    public class MessageService : IMessageService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IMessageRepository _messageRepository;
        public MessageService(IConversationRepository conversationRepository, IMessageRepository messageRepository)
        {
            _conversationRepository = conversationRepository;
            _messageRepository = messageRepository;
        }
        public async Task<List<Message>> GetMessagesAsync(string WaId)
        {
            var conversations = await _conversationRepository.GetListConversationsByWaIDPhoneAsync(WaId);

            var messages = new List<Message>(); // Return the list of conversations

            foreach (var conversation in conversations)
            {
                messages.AddRange(conversation.Messages);
            }

            return messages;
        }
    }
}
