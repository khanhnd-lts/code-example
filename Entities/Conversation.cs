using CMS.Core.Features;
using CMS.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CMS.Core.Entities.Chat
{
    public  class Conversation:BaseEntity, ISoftDeletable
    {
        public string Title { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public string AccountDelete { get; set; }
        public virtual IEnumerable<ChatMessage> ChatMessages { get; set; }
        public virtual IEnumerable<Participant> Participants { get; set; }

        [NotMapped]
        public ChatMessage LastChatMessage { get; set; }
        [NotMapped]
        public int? TotalUnSeenChatMessages { get; set; }
    }
}
