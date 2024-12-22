using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonoVik.WebApi.Users.Infrastructure
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.UserId).HasName("users_pkey");

            builder.ToTable("users");

            builder.HasIndex(e => e.Email, "users_email_key").IsUnique();

            builder.HasIndex(e => e.Username, "users_username_key").IsUnique();

            builder.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            builder.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
            builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            builder.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            builder.Property(e => e.LastSeen)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("last_seen");
            builder.Property(e => e.IsVerified)
                .HasDefaultValue(false)
                .HasColumnName("is_verified");
            builder.Property(e => e.PasswordHash)
                .HasMaxLength(100)
                .HasColumnName("password_hash");
            builder.Property(e => e.StatusMessage)
                .HasMaxLength(255)
                .HasColumnName("status_message");
            builder.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");
            builder.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        }
    }
}
