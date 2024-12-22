using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonoVik.WebApi.UserPreferences.Infrastructure
{
    public sealed class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
    {
        public void Configure(EntityTypeBuilder<UserPreference> builder)
        {
            builder.HasKey(e => e.UserId).HasName("user_preferences_pkey");

            builder.ToTable("user_preferences");

            builder.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            builder.Property(e => e.NotificationSound)
                .HasDefaultValue(true)
                .HasColumnName("notification_sound");
            builder.Property(e => e.PrivacyLevel)
                .HasMaxLength(20)
                .HasDefaultValueSql("'friends_only'::character varying")
                .HasColumnName("privacy_level");
            builder.Property(e => e.ReceiveNotifications)
                .HasDefaultValue(true)
                .HasColumnName("receive_notifications");
            builder.Property(e => e.Theme)
                .HasMaxLength(20)
                .HasDefaultValueSql("'light'::character varying")
                .HasColumnName("theme");

            builder.HasOne(d => d.User).WithOne(p => p.UserPreference)
                .HasForeignKey<UserPreference>(d => d.UserId)
                .HasConstraintName("user_preferences_user_id_fkey");
        }
    }
}
