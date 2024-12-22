using MonoVik.WebApi.Users;

namespace MonoVik.WebApi.UserBlocks
{
    public class UserBlock
    {
        public Guid BlockerId { get; set; }

        public Guid BlockedId { get; set; }

        public DateTime? BlockedAt { get; set; }

        public virtual User Blocked { get; set; } = null!;

        public virtual User Blocker { get; set; } = null!;
    }
}
