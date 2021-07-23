using CMS.Core.Entities;
using CMS.Core.Entities.Chat;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CMS.Core.Interfaces.Services
{
    public interface IConversationService
    {
        public IQueryable<Conversation> GetConversations(string account = null, string keywords = null);
        public Task<Conversation> GetConversationById(int id);
        public Task<Conversation> CreateConversation(Conversation conversation, string creatorAccount);
        public Task UpdateConversation(Conversation conversation);
        public Task DeleteConversation(int id, string accountDelete);

        public IQueryable<ChatMessage> GetChatMessages(int conversationId, string account);
        public Task<ChatMessage> GetChatMessageById(int id, string account);
        public Task<ChatMessage> CreateChatMessage(ChatMessage chatMessage, string creatorAccount);
        public Task DeleteChatMessage(int chatMessageId, string accountDelete);
        public Task MarkSeenChatMessages(IEnumerable<int> chatMessageIds, string account);
        public Task MarkSeenChatMessage(int chatMessageId, string account);
        public Task MarkUnSeenChatMessage(int chatMessageId, string account);

        public Task<int> GetTotalUnSeenChatMessages(string account, int? conversationId = null);
        public IQueryable<Participant> GetParticipants(int conversationId);
        public Task DeleteAllDataByAccount(string account);
    }
}