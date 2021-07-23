using CMS.Core.Features;
using CMS.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Core.Entities.Chat
{
    public class ChatMessageParticipantState: BaseEntity
    {
        public int ChatMessageId { get; set; }
        public bool Seen { get; set; }

        public DateTime? SeenTime { get; set; }
        public int ParticipantId { get; set; }
        public virtual ChatMessage ChatMessage { get; set; }
        public virtual Participant Participant { get; set; }
    }
}
