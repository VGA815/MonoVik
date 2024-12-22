using MonoVik.WebApi.Chats;
using MonoVik.WebApi.Users;

namespace MonoVik.WebApi.ChatMembers
{
    public class ChatMember
    {
        public Guid ChatId { get; set; }

        public Guid UserId { get; set; }

        public string? Role { get; set; }

        public bool? CanPost { get; set; }

        public bool? CanAddMembers { get; set; }

        public bool? CanRemoveMembers { get; set; }

        public DateTime? JoinedAt { get; set; }

        public DateTime? LeftAt { get; set; }

        public virtual Chat Chat { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
