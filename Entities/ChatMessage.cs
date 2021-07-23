using CMS.Core.Features;
using CMS.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Core.Entities.Chat
{
    public class ChatMessage: BaseEntity, ISoftDeletable
    {
        public int ConversationId { get; set; }
        public string Content { get; set; }
        public string Photo { get; set; }
        public string File { get; set; }
        public int CreatorId { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public string AccountDelete { get; set; }
        public virtual Participant Creator { get; set; }
        public virtual Conversation Conversations { get; set; }
        public virtual IEnumerable<ChatMessageParticipantState> ChatMessageParticipantStates { get; set; }
    }
}
