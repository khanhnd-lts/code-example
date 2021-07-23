using CMS.Core.Features;
using CMS.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Core.Entities.Chat
{
    public class Participant: BaseEntity
    {
        public int ConversationId { get; set; }
        [MaxLength(150)]
        public string Account { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool IsCreator { get; set; }
        public virtual HocVien HocVien { get; set; }
        public virtual Conversation Conversations { get; set; }
        public virtual IEnumerable<ChatMessageParticipantState> ChatMessageParticipantStates { get; set; }
        public virtual IEnumerable<ChatMessage> ChatMessages { get; set; }

    }
}
