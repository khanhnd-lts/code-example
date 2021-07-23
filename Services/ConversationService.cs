using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Core.Entities;
using CMS.Core.Entities.Chat;
using CMS.Core.Extensions;
using CMS.Core.Interfaces;
using CMS.Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace CMS.Core.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IRepository<Conversation> _conversationRepository;
        private readonly IRepository<ChatMessage> _chatMessageRepository;
        private readonly IRepository<Participant> _participantRepository;
        private readonly IRepository<ChatMessageParticipantState> _chatMessageParticipantStateRepository;
        private readonly IRepository<HocVien> _hocVienRepository;
        private readonly IRepository<NhanVien> _nhanVienRepository;
        public ConversationService(IRepository<Conversation> conversationRepository,
                             IRepository<ChatMessage> chatMessageRepository,
                             IRepository<Participant> participantRepository,
                             IRepository<ChatMessageParticipantState> chatMessageParticipantStateRepository,
                             IRepository<HocVien> hocVienRepository,
                             IRepository<NhanVien> nhanVienRepository)
        {
            _conversationRepository = conversationRepository;
            _chatMessageRepository = chatMessageRepository;
            _participantRepository = participantRepository;
            _chatMessageParticipantStateRepository = chatMessageParticipantStateRepository;
            _hocVienRepository = hocVienRepository;
            _nhanVienRepository = nhanVienRepository;
        }

        public async Task<ChatMessage> CreateChatMessage(ChatMessage chatMessage, string creatorAccount)
        {
            if (!_conversationRepository.TableUntracked.Any(x => x.Id == chatMessage.ConversationId))
                return null;
            var creator = await _participantRepository
                .TableUntracked
                .FirstOrDefaultAsync(x => x.Account == creatorAccount
                    && x.ConversationId == chatMessage.ConversationId);
            if (creator == null)
                return null;
            chatMessage.CreatedTime = DateTime.Now;
            chatMessage.Deleted = false;
            chatMessage.CreatorId = creator.Id;
            chatMessage.ChatMessageParticipantStates = new List<ChatMessageParticipantState>
            {
                new ChatMessageParticipantState
                {
                    ParticipantId = creator.Id,
                    Seen = true,
                    SeenTime = DateTime.Now
                }
            };
            await _chatMessageRepository.AddAsync(chatMessage);
            chatMessage.Creator = await _participantRepository.TableUntracked
                .Include(x=>x.HocVien)
                .FirstOrDefaultAsync(x => x.Id == chatMessage.CreatorId);
            return chatMessage;
        }

        public async Task<Conversation> CreateConversation(Conversation conversation, string creatorAccount)
        {
            if (conversation.Participants.IsNullOrEmpty())
                return null;

            if (!conversation.Participants.Any(x => x.Account == creatorAccount))
            {
                conversation.Participants.AddRange(new List<Participant> {
                        new Participant {
                            Account=creatorAccount,
                            IsCreator = true,
                            CreatedTime = DateTime.Now
                        }
                    });
            }
            if (conversation.Participants.Count() == 2)
            {
                var otherAccount = conversation.Participants
                             .FirstOrDefault(z => z.Account != creatorAccount).Account;
                var conversationOld = await _conversationRepository.TableUntracked
                     .Include(x => x.Participants)
                     .ThenInclude(x => x.HocVien)
                     //.Include(x => x.ChatMessages)
                     //.ThenInclude(x => x.ChatMessageParticipantStates)
                     //.Include(x => x.ChatMessages)
                     //.ThenInclude(x => x.Creator)
                     //.ThenInclude(x => x.HocVien)
                     .FirstOrDefaultAsync(x => x.Participants.Count() == 2
                         && x.Participants.Any(y => y.Account == creatorAccount)
                         && x.Participants.Any(y => y.Account == otherAccount));
                if (conversationOld != null)
                    conversation = conversationOld;
                else
                {
                    foreach (var participant in conversation.Participants)
                    {
                        participant.CreatedTime = DateTime.Now;
                    }
                    conversation.Creator = creatorAccount;
                    conversation.CreatedTime = DateTime.Now;
                    conversation.Deleted = false;
                    await _conversationRepository.AddAsync(conversation);
                    conversation = await GetConversationById(conversation.Id);
                }
            }
            else
            {
                var conversationOld = await _conversationRepository.TableUntracked
                    .Include(x => x.Participants)
                    .ThenInclude(x => x.HocVien)
                    .FirstOrDefaultAsync(x => x.Participants.Count() == 1
                        && x.Participants.Any(y => y.Account == creatorAccount));
                if (conversationOld != null)
                    conversation = conversationOld;
                else
                {
                    foreach (var participant in conversation.Participants)
                    {
                        participant.CreatedTime = DateTime.Now;
                    }
                    conversation.Creator = creatorAccount;
                    conversation.CreatedTime = DateTime.Now;
                    conversation.Deleted = false;
                    await _conversationRepository.AddAsync(conversation);
                    conversation = await GetConversationById(conversation.Id);
                }
            }
            return conversation;
        }

        public async Task DeleteAllDataByAccount(string account)
        {
            var danhSachHoiThoaiCuaToi = _conversationRepository.TableUntracked
                                                                .Include(x => x.Participants)
                                                                .Where(x => x.Participants.Any(y => y.Account == account));
            if (danhSachHoiThoaiCuaToi != null) await _conversationRepository.DeleteRangeAsync(danhSachHoiThoaiCuaToi);
        }

        public async Task DeleteChatMessage(int chatMessageId, string accountDelete)
        {
            var chatMessage = await _chatMessageRepository.TableUntracked.FirstOrDefaultAsync(x => x.Id == chatMessageId);
            if (chatMessage == null)
                return;
            chatMessage.AccountDelete = accountDelete;
            await _chatMessageRepository.DeleteAsync(chatMessage);
        }

        public async Task DeleteConversation(int id, string accountDelete)
        {
            var conversation = await _conversationRepository.TableUntracked
                .FirstOrDefaultAsync(x => x.Id == id);
            if (conversation == null)
                return;
            conversation.AccountDelete = accountDelete;
            await _conversationRepository.DeleteAsync(conversation);
        }

        public async Task<ChatMessage> GetChatMessageById(int chatMessageId, string account)
        {
            var chatMessage = await _chatMessageRepository
                .TableUntracked.FirstOrDefaultAsync(x => x.Id == chatMessageId);
            if (chatMessage == null)
                return null;
            var conversation = await _conversationRepository.TableUntracked
                   .FirstOrDefaultAsync(x => x.Id == chatMessage.ConversationId);
            if (conversation == null)
                return null;
            var participant = await _participantRepository
                .TableUntracked
                .FirstOrDefaultAsync(x => x.Account == account
                    && x.ConversationId == conversation.Id);
            if (participant == null)
                return null;
            return chatMessage;
        }

        public IQueryable<ChatMessage> GetChatMessages(int conversationId, string account)
        {
            var conversation = _conversationRepository.TableUntracked
                   .FirstOrDefault(x => x.Id == conversationId);
            if (conversation == null)
                return null;
            var participant = _participantRepository
                .TableUntracked
                .FirstOrDefault(x => x.Account == account
                    && x.ConversationId == conversation.Id);
            if (participant == null)
                return null;
            var chatMessages = _chatMessageRepository
                .TableUntracked
                .Include(x => x.ChatMessageParticipantStates)
                .Include(x => x.Creator)
                .ThenInclude(x => x.HocVien)
                .Where(x => x.ConversationId == conversationId);

            return chatMessages;
        }

        public async Task<Conversation> GetConversationById(int id)
        {
            var conversation = await _conversationRepository.TableUntracked
                     .Include(x => x.Participants)
                     .ThenInclude(x => x.HocVien)
                     //.Include(x => x.ChatMessages)
                     //.ThenInclude(x => x.ChatMessageParticipantStates)
                     //.Include(x => x.ChatMessages)
                     //.ThenInclude(x => x.Creator)
                     //.ThenInclude(x => x.HocVien)
                .FirstOrDefaultAsync(x => x.Id == id);
            return conversation;
        }

        public IQueryable<Conversation> GetConversations(string account = null, string keywords = null)
        {
            var query = _conversationRepository.TableUntracked
                .Include(x => x.Participants)
                .ThenInclude(x => x.HocVien)
                //.Include(x=>x.ChatMessages)
                //.ThenInclude(x => x.ChatMessageParticipantStates)
                //.ThenInclude(x => x.Participant)
                .AsNoTracking();
            if (!account.IsNullOrEmpty())
            {
                query = query.Where(x => x.Participants.Any(y => y.Account == account));
            }
            if (!keywords.IsNullOrEmpty())
            {
                query = query.Where(x => x.Title.Contains(keywords)
                || x.Participants.Any(y => y.Account.Contains(keywords)));
            }
            return query.Select(x=>new Conversation
            {
                Id = x.Id,
                Title = x.Title,
                AccountDelete = x.AccountDelete,
                CreatedTime = x.CreatedTime,
                Creator = x.Creator,
                Deleted = x.Deleted,
                DeletedTime = x.DeletedTime,
                Participants = x.Participants.ToList(),
                TotalUnSeenChatMessages = _chatMessageRepository.TableUntracked
                    .Include(y=>y.ChatMessageParticipantStates)
                    .ThenInclude(y=>y.Participant)
                    .Count(y=>y.ConversationId == x.Id && !y.ChatMessageParticipantStates
                        .Any(z => z.Seen && z.Participant.Account == account)),
                LastChatMessage = _chatMessageRepository
                    .TableUntracked.Where(y => y.ConversationId == x.Id)
                    .OrderByDescending(y=>y.CreatedTime)
                    .FirstOrDefault()
            });
        }

        public IQueryable<Participant> GetParticipants(int conversationId)
        {
            var query = _participantRepository
                .TableUntracked
                .Where(x => x.ConversationId == conversationId);
            return query;
        }

        public async Task<int> GetTotalUnSeenChatMessages(string account, int? conversationId = null)
        {
            var query = _chatMessageRepository
                .TableUntracked
                .Where(x => x.Conversations.Participants.Any(y => y.Account == account)
                && !x.ChatMessageParticipantStates.Any(y => y.Seen && y.Participant.Account == account));
            if (conversationId.HasValue)
                query = query
                    .Where(x => x.ConversationId == conversationId);
            var totalUnSeenChatMessages = await query.CountAsync();
            return totalUnSeenChatMessages;
        }

        public async Task MarkSeenChatMessage(int chatMessageId, string account)
        {
            var chatMessage = await _chatMessageRepository
                  .TableUntracked.FirstOrDefaultAsync(x => x.Id == chatMessageId);
            if (chatMessage == null)
                return;
            var conversation = await _conversationRepository.TableUntracked
                .FirstOrDefaultAsync(x => x.Id == chatMessage.ConversationId);
            if (conversation == null)
                return;
            var participant = await _participantRepository
                .TableUntracked
                .FirstOrDefaultAsync(x => x.Account == account
                    && x.ConversationId == conversation.Id);
            if (participant == null)
                return;
            var chatMessageState = await _chatMessageParticipantStateRepository
                .TableUntracked.FirstOrDefaultAsync(x => x.ChatMessageId == chatMessageId
                    && x.ParticipantId == participant.Id);
            if (chatMessageState == null)
            {
                chatMessageState = new ChatMessageParticipantState
                {
                    ParticipantId = participant.Id,
                    Seen = true,
                    SeenTime = DateTime.Now,
                    ChatMessageId = chatMessageId
                };
                await _chatMessageParticipantStateRepository.AddAsync(chatMessageState);
            }
            else
            {
                chatMessageState.Seen = true;
                chatMessageState.SeenTime = DateTime.Now;
                await _chatMessageParticipantStateRepository.UpdateAsync(chatMessageState);
            }
        }

        public async Task MarkSeenChatMessages(IEnumerable<int> chatMessageIds, string account)
        {
            var chatMessages = _chatMessageRepository
                     .TableUntracked.Where(x => chatMessageIds.Contains(x.Id))
                     .ToList();
            if (!chatMessages.Any())
                return;
            var chatMessageStatesUpdate = new List<ChatMessageParticipantState>();
            var chatMessageStatesAdd = new List<ChatMessageParticipantState>();
            foreach (var chatMessage in chatMessages)
            {
                var conversation = await _conversationRepository.TableUntracked
                    .FirstOrDefaultAsync(x => x.Id == chatMessage.ConversationId);
                if (conversation == null)
                    return;
                var participant = await _participantRepository
                    .TableUntracked
                    .FirstOrDefaultAsync(x => x.Account == account
                        && x.ConversationId == conversation.Id);
                if (participant == null)
                    return;
                var chatMessageState = await _chatMessageParticipantStateRepository
                    .TableUntracked.FirstOrDefaultAsync(x => x.ChatMessageId == chatMessage.Id
                        && x.ParticipantId == participant.Id);
                if (chatMessageState == null)
                {
                    chatMessageState = new ChatMessageParticipantState
                    {
                        ParticipantId = participant.Id,
                        Seen = true,
                        SeenTime = DateTime.Now,
                        ChatMessageId = chatMessage.Id
                    };
                    chatMessageStatesAdd.Add(chatMessageState);
                }
                else if(!chatMessageState.Seen)
                {
                    chatMessageState.Seen = true;
                    chatMessageState.SeenTime = DateTime.Now;
                    chatMessageStatesUpdate.Add(chatMessageState);
                }
            }

            await _chatMessageParticipantStateRepository.AddRangeAsync(chatMessageStatesAdd);
            await _chatMessageParticipantStateRepository.UpdateRangeAsync(chatMessageStatesUpdate);
        }

        public async Task MarkUnSeenChatMessage(int chatMessageId, string account)
        {
            var chatMessage = await _chatMessageRepository
                  .TableUntracked.FirstOrDefaultAsync(x => x.Id == chatMessageId);
            if (chatMessage == null)
                return;
            var conversation = await _conversationRepository.TableUntracked
                   .FirstOrDefaultAsync(x => x.Id == chatMessage.ConversationId);
            if (conversation == null)
                return;
            var participant = await _participantRepository
                .TableUntracked
                .FirstOrDefaultAsync(x => x.Account == account
                    && x.ConversationId == conversation.Id);
            if (participant == null)
                return;
            var chatMessageState = await _chatMessageParticipantStateRepository
                .TableUntracked.FirstOrDefaultAsync(x => x.ChatMessageId == chatMessageId
                    && x.ParticipantId == participant.Id);
            if (chatMessageState == null)
            {
                chatMessageState = new ChatMessageParticipantState
                {
                    ParticipantId = participant.Id,
                    Seen = false,
                    ChatMessageId = chatMessageId
                };
                await _chatMessageParticipantStateRepository.AddAsync(chatMessageState);
            }
            else
            {
                chatMessageState.Seen = false;
                await _chatMessageParticipantStateRepository.UpdateAsync(chatMessageState);
            }
        }

        public async Task UpdateConversation(Conversation conversation)
        {
            await _conversationRepository.UpdateAsync(conversation);
        }
    }
}