using MonoVik.WebApi.Chats;
using MonoVik.WebApi.Users;

namespace MonoVik.WebApi.Messages
{
    public class Message
    {
        public Guid MessageId { get; set; }

        public Guid? ChatId { get; set; }

        public Guid? SenderId { get; set; }

        public string Content { get; set; } = null!;

        public string? AttachmentUrl { get; set; }

        public bool? IsEdited { get; set; }

        public bool? IsDeleted { get; set; }

        public List<string>? Tags { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual Chat? Chat { get; set; }

        public virtual User? Sender { get; set; }
    }
}
