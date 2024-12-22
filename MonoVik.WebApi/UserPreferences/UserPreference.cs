using MonoVik.WebApi.Users;

namespace MonoVik.WebApi.UserPreferences
{
    public class UserPreference
    {
        public Guid UserId { get; set; }

        public bool? ReceiveNotifications { get; set; }

        public bool? NotificationSound { get; set; }

        public string? Theme { get; set; }

        public string? PrivacyLevel { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
