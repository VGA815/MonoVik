using MonoVik.WebApi.ChatMembers;
using MonoVik.WebApi.Messages;

namespace MonoVik.WebApi.Chats
{
    public class Chat
    {
        public Guid ChatId { get; set; }

        public string ChatName { get; set; } = null!;

        public string? ChatDescription { get; set; }

        public string? ChatImageUrl { get; set; }

        public bool? IsGroup { get; set; }

        public string? ChatType { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<ChatMember> ChatMembers { get; set; } = new List<ChatMember>();

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
