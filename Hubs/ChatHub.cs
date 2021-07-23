using CMS.Core.Entities.Chat;
using CMS.Core.Enums;
using CMS.Core.Interfaces.Services;
using CMS.Infrastructure;
using CMS.Infrastructure.Data;
using CMS.Web.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Web.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConversationService _conversationService;
        private readonly IHocVienService _hocVienService;
        private static List<string> _usersOnline = new List<string>();
        private static List<string> _usersShareScreen = new List<string>();
        private static List<ConnectionShare> _connectionsShare = new List<ConnectionShare>();
        private static Dictionary<string, SignalScreenMonitoringModel> _signals = new Dictionary<string, SignalScreenMonitoringModel>();
        public ChatHub(IConversationService conversationService,
            UserManager<ApplicationUser> userManager,
            IHocVienService hocVienService)
        {
            _conversationService = conversationService;
            _userManager = userManager;
            _hocVienService = hocVienService;
        }

        #region Chat

        [Authorize]
        public override async Task OnConnectedAsync()
        {
            // Todo: manage connections connected
            await Groups.AddToGroupAsync(Context.ConnectionId, "ConnectedClients");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{Context.User.Identity.Name}");
            lock (_usersOnline)
            {
                if (!_usersOnline.Contains(Context.User.Identity.Name))
                    _usersOnline.Add(Context.User.Identity.Name);
            }
            lock (_connectionsShare)
            {
                _connectionsShare.Add(new ConnectionShare() { Account = Context.User.Identity.Name, ConnectionId = Context.ConnectionId, IsSharing = false });
            }
            await base.OnConnectedAsync();

            var supporterContacts = await getSupporterContacts();
            await Clients.Client(Context.ConnectionId).SendAsync("PushSupporterContacts", supporterContacts);

            await GetTotalUnSeenChatMessages();
            try
            {
                var queryConversations = _conversationService
                    .GetConversations(Context.User.Identity.Name)
                    .ToList()
                    .Select(x => ConversationDTO.FromEntity(x, Context.User.Identity.Name, false))
                    .OrderByDescending(x => x.TotalUnSeenChatMessages)
                    .ThenByDescending(x => x.LastChatMessage?.CreatedTime);

                var pagination = new Pagination
                {
                    Page = 0,
                    ItemsPerPage = 10
                };
                var conversations = PagedList
                    .Create(queryConversations, pagination.Page, pagination.ItemsPerPage);

                var conversationDTOs = new PagedResult<ConversationDTO>(pagination, conversations);
                await Clients.Client(Context.ConnectionId).SendAsync("PushConversations", conversationDTOs);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            await Clients.All.SendAsync("PushUserOnline", Context.User.Identity.Name);
            await Clients.Caller.SendAsync("PushUsersOnline", _usersOnline);
            var user = await _userManager.FindByNameAsync(Context.User.Identity.Name);
            if (await _userManager.IsInRoleAsync(user, Roles.ADMIN) || await _userManager.IsInRoleAsync(user, Roles.TRO_GIANG))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Roles.ADMIN_TRO_GIANG);
            }
            var usersOnline = _hocVienService.GetHocVienOnline(_usersOnline)
                .Select(x => new HocVienDTO
                {
                    Id = x.Id,
                    HoTen = x.HoTen,
                    Account = x.Account,
                    LinkAnhCaNhan = x.LinkAnhCaNhan,
                    Email = x.Email,
                    NgaySinh = x.NgaySinh,
                    Sdt = x.Sdt,
                    DiaChi = x.DiaChi
                });
            var usersShareScreen = usersOnline.Where(x => _usersShareScreen.Contains(x.Account));
            await Clients.Group(Roles.ADMIN_TRO_GIANG).SendAsync("PushUsersOnlineAdmin", usersOnline);
            await Clients.Group(Roles.ADMIN_TRO_GIANG).SendAsync("PushUsersShareScreen", usersShareScreen);
        }

        [Authorize]
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Todo: manage connections disconnected
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "ConnectedClients");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{Context.User.Identity.Name}");
            lock (_usersOnline)
            {
                if (_usersOnline.Contains(Context.User.Identity.Name))
                    _usersOnline.Remove(Context.User.Identity.Name);
            }
            lock (_usersShareScreen)
            {
                if (_usersShareScreen.Contains(Context.User.Identity.Name))
                    _usersShareScreen.Remove(Context.User.Identity.Name);
            }
            lock (_connectionsShare)
            {
                var connection = _connectionsShare.FirstOrDefault(x => x.Account == Context.User.Identity.Name && x.ConnectionId == Context.ConnectionId);
                if (connection != null) _connectionsShare.Remove(connection);
            }
            try
            {
                await Clients.All.SendAsync("PushUserOffline", Context.User.Identity.Name);
                var user = await _userManager.FindByNameAsync(Context.User.Identity.Name);
                if (await _userManager.IsInRoleAsync(user, Roles.ADMIN) || await _userManager.IsInRoleAsync(user, Roles.TRO_GIANG))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, Roles.ADMIN_TRO_GIANG);
                }
                var usersOnline = _hocVienService.GetHocVienOnline(_usersOnline)
                    .Select(x => new HocVienDTO
                    {
                        Id = x.Id,
                        HoTen = x.HoTen,
                        Account = x.Account,
                        LinkAnhCaNhan = x.LinkAnhCaNhan,
                        Email = x.Email,
                        NgaySinh = x.NgaySinh,
                        Sdt = x.Sdt,
                        DiaChi = x.DiaChi
                    });
                await Clients.Group(Roles.ADMIN_TRO_GIANG).SendAsync("PushUsersOnlineAdmin", usersOnline);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            await base.OnDisconnectedAsync(exception);
        }

        [Authorize]
        public async Task MarkSeenChatMessage(int chatMessageId)
        {
            await _conversationService.MarkSeenChatMessage(chatMessageId, Context.User.Identity.Name);

            var chatMessage = await _conversationService.GetChatMessageById(chatMessageId, Context.User.Identity.Name);
            var participants = _conversationService.GetParticipants(chatMessage.ConversationId);
            foreach (var participant in participants)
            {
                await Clients.Groups($"User_{participant.Account}").SendAsync("PushChatMessageState", ChatMessageDTO.FromEntity(chatMessage));
            }
        }
        [Authorize]
        public async Task<ChatMessageDTO> SendMessage(int conversationId, string content, string photo = null, string file = null)
        {
            var chatMessage = new ChatMessage
            {
                ConversationId = conversationId,
                Content = content,
                File = file,
                Photo = photo
            };
            chatMessage = await _conversationService.CreateChatMessage(chatMessage, Context.User.Identity.Name);

            var participants = _conversationService.GetParticipants(chatMessage.ConversationId);

            var result = ChatMessageDTO.FromEntity(chatMessage);
            foreach (var participant in participants)
            {
                await Clients.GroupExcept($"User_{participant.Account}", Context.ConnectionId).SendAsync("PushNewChatMessage", result);
            }
            return result;
        }
        [Authorize]
        public async Task AddOrOpenConversation(Conversation conversation)
        {
            if (conversation.Id != 0)
                conversation = await _conversationService.GetConversationById(conversation.Id);
            else
            {
                conversation = await _conversationService.CreateConversation(conversation, Context.User.Identity.Name);
            }
            conversation.ChatMessages = _conversationService.GetChatMessages(conversation.Id, Context.User.Identity.Name);
            var result = ConversationDTO.FromEntity(conversation, Context.User.Identity.Name);
            await Clients.Caller.SendAsync("PushAddOrOpenConversation", result);
            await _conversationService
                .MarkSeenChatMessages(conversation.ChatMessages.Select(x => x.Id).ToList(), Context.User.Identity.Name);
        }
        [Authorize]
        public async Task GetChatMessages(int conversationId)
        {
            var pagination = new Pagination
            {
                Page = 0,
                ItemsPerPage = -1
            };
            var query = _conversationService.GetChatMessages(conversationId, Context.User.Identity.Name);
            var chatMessages = PagedList.Create(query, pagination.Page, pagination.ItemsPerPage);
            pagination.TotalItems = chatMessages.TotalCount;
            var chatMessagesDTOs = new PagedResult<ChatMessageDTO>(pagination, chatMessages.Select(ChatMessageDTO.FromEntity));

            await Clients.Groups($"User_{Context.User.Identity.Name}").SendAsync("PushChatMessages", chatMessagesDTOs);

            await _conversationService
                .MarkSeenChatMessages(chatMessagesDTOs.Data.Select(x => x.Id), Context.User.Identity.Name);
        }
        private async Task<PagedResult<HocVienDTO>> getSupporterContacts()
        {
            var queryUsers = await _userManager.Users
                .AsNoTracking()
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Where(x => x.UserRoles.Any(x => Roles.TRO_GIANG == x.Role.Name))
                .Select(x => x.UserName)
                .ToListAsync();
            var queryHocVien = _hocVienService.GetHocVien()
                .Where(x => queryUsers.Contains(x.Account));

            var pagination = new Pagination
            {
                Page = 0,
                ItemsPerPage = -1
            };
            var danhSachTroGiang = PagedList.Create(queryHocVien, pagination.Page, pagination.ItemsPerPage);
            pagination.TotalItems = danhSachTroGiang.TotalCount;
            var danhSachTroGiangDTO = new PagedResult<HocVienDTO>(pagination, danhSachTroGiang.Select(HocVienDTO.FromEntity));
            return danhSachTroGiangDTO;
        }
        [Authorize]
        public async Task GetTotalUnSeenChatMessages()
        {
            var totalMessages = await _conversationService.GetTotalUnSeenChatMessages(Context.User.Identity.Name);
            await Clients.Groups($"User_{Context.User.Identity.Name}").SendAsync("PushTotalUnSeenChatMessages", totalMessages);
        }
        [Authorize]
        public async Task GetContacts(string keywords)
        {
            var query = _hocVienService.GetHocVien(keywords);
            var pagination = new Pagination
            {
                Page = 0,
                ItemsPerPage = 10
            };
            var contacts = PagedList.Create(query, pagination.Page, pagination.ItemsPerPage);
            pagination.TotalItems = contacts.TotalCount;
            var contactDTOs = new PagedResult<HocVienDTO>(pagination, contacts.Select(HocVienDTO.FromEntity));
            await Clients.Caller.SendAsync("PushContacts", contactDTOs);
        }

        #endregion

        #region Screen monitoring
        public class ConnectionShare
        {
            public string Account { get; set; }
            public string ConnectionId { get; set; }
            public bool IsSharing { get; set; }
        }
        public class SignalScreenMonitoringModel
        {
            //public string FromConnectionId { get; set; }
            //public string ToConnectionId { get; set; }
            public string Action { get; set; }
            public SignalData SignalData { get; set; }
        }
        public class SignalData
        {
            public string From { get; set; }
            public string To { get; set; }
            public string Sdp { get; set; }
            public string Type { get; set; }
            public SignalDataCandidate Candidate { get; set; }
            public SignalTransceiverRequest TransceiverRequest { get; set; }
            public bool? Renegotiate { get; set; }
        }
        public class SignalDataCandidate
        {
            public string Candidate { get; set; }
            public int SdpMLineIndex { get; set; }
            public string SdpMid { get; set; }
        }
        public class SignalTransceiverRequest
        {
            public object init { get; set; }
            public string Kind { get; set; }
        }
        public class VoiceCallModel
        {
            public string From { get; set; }
            public string To { get; set; }
            public bool AutoAcceptCall { get; set; }
        }
        [Authorize]
        public async Task StartVoiceCall(VoiceCallModel voiceCallModel)
        {
            voiceCallModel.From = Context.User.Identity.Name;
            await Clients.Group($"User_{voiceCallModel.To}").SendAsync("VoiceCalling", voiceCallModel);
        }
        [Authorize]
        public async Task AcceptVoiceCalling(VoiceCallModel voiceCallModel)
        {
            await Clients.Group($"User_{voiceCallModel.From}").SendAsync("VoiceCallingAccepted", voiceCallModel);
        }
        [Authorize]
        public async Task RejectVoiceCalling(VoiceCallModel voiceCallModel)
        {
            await Clients.Group($"User_{voiceCallModel.From}").SendAsync("VoiceCallingRejected", voiceCallModel);
            await Clients.Group($"User_{voiceCallModel.To}").SendAsync("VoiceCallingRejected", voiceCallModel);
        }
        [Authorize]
        public async Task StopVoiceCalling(VoiceCallModel voiceCallModel)
        {
            await Clients.Group($"User_{voiceCallModel.From}").SendAsync("VoiceCallingStoped", voiceCallModel);
            await Clients.Group($"User_{voiceCallModel.To}").SendAsync("VoiceCallingStoped", voiceCallModel);
        }
        [Authorize]
        public async Task SignalToUser(SignalScreenMonitoringModel signalScreenMonitoringModel)
        {
            signalScreenMonitoringModel.SignalData.From = Context.User.Identity.Name;
            if (signalScreenMonitoringModel.SignalData.Type == "offer")
            {
                var connection = _connectionsShare.FirstOrDefault(x => x.Account == signalScreenMonitoringModel.SignalData.To && x.IsSharing);
                if (connection != null)
                    await Clients.Client(connection.ConnectionId).SendAsync("StartScreenMonitoring", signalScreenMonitoringModel);
            }
            else
                await Clients.Group($"User_{signalScreenMonitoringModel.SignalData.To}").SendAsync("StartScreenMonitoring", signalScreenMonitoringModel);
        }
        [Authorize]
        public async Task StartShareScreen(HocVienDTO hocVien)
        {
            lock (_usersShareScreen)
            {
                if (!_usersShareScreen.Contains(Context.User.Identity.Name))
                    _usersShareScreen.Add(Context.User.Identity.Name);
            }
            lock (_connectionsShare)
            {
                var connection = _connectionsShare.FirstOrDefault(x => x.Account == Context.User.Identity.Name && x.ConnectionId == Context.ConnectionId);
                if (connection != null) connection.IsSharing = true;
            }
            await Clients.Group(Roles.ADMIN_TRO_GIANG).SendAsync("OnStartShareScreen", hocVien);
        }
        [Authorize]
        public async Task StopShareScreen(HocVienDTO hocVien)
        {
            lock (_usersShareScreen)
            {
                if (_usersShareScreen.Contains(Context.User.Identity.Name))
                    _usersShareScreen.Remove(Context.User.Identity.Name);
            }
            lock (_connectionsShare)
            {
                var connection = _connectionsShare.FirstOrDefault(x => x.Account == Context.User.Identity.Name && x.ConnectionId == Context.ConnectionId && x.IsSharing);
                if (connection != null) connection.IsSharing = false;
            }
            await Clients.Group(Roles.ADMIN_TRO_GIANG).SendAsync("OnStopShareScreen", hocVien);
        }

        [Authorize]
        public async Task WatchScreenShare(string account)
        {
            await Clients.Group($"User_{account}").SendAsync("OnWatchScreenShare", Context.User.Identity.Name);
        }
        [Authorize]
        public async Task RequestDisconnection(string account)
        {
            await Clients.Group($"User_{account}").SendAsync("OnRequestDisconnection", Context.User.Identity.Name);
        }
        [Authorize]
        public async Task RequestOtherDisconnection(string account)
        {
            await Clients.Group($"User_{account}").SendAsync("OnRequestOtherDisconnection");
        }
        [Authorize]
        public async Task ResponseDisconnection(string account)
        {
            await Clients.Group($"User_{account}").SendAsync("OnResponseDisconnection");
        }
        #endregion
    }
}
