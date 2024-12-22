using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonoVik.WebApi.Users.Infrastructure
{
    public sealed class EmailVerificationTokenConfiguration : IEntityTypeConfiguration<EmailVerificationToken>
    {
        public void Configure(EntityTypeBuilder<EmailVerificationToken> builder)
        {
            builder.HasKey(e => e.TokenId).HasName("email_verification_tokens_pkey");

            builder.ToTable("email_verification_tokens");

            builder.HasIndex(e => e.UserId, "idx_email_verification_tokens_user_id");

            builder.Property(e => e.TokenId)
                .ValueGeneratedNever()
                .HasColumnName("token_id");
            builder.Property(e => e.UserId)
                .HasColumnName("user_id");
            builder.Property(e => e.ExpiresAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("expires_at");
            builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            builder.HasOne(d => d.User).WithOne(e => e.EmailVerificationToken)
                .HasConstraintName("email_verification_tokens_user_id_fkey");
        }
    }
}
