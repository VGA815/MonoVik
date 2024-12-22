namespace MonoVik.WebApi.Users
{
    public class EmailVerificationToken
    {
        public Guid TokenId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
