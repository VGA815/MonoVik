using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonoVik.WebApi.ChatMembers.Infrastructure
{
    public sealed class ChatMemberConfiguration : IEntityTypeConfiguration<ChatMember>
    {
        public void Configure(EntityTypeBuilder<ChatMember> builder)
        {
            builder.HasKey(e => new { e.ChatId, e.UserId }).HasName("chat_members_pkey");

            builder.ToTable("chat_members");

            builder.Property(e => e.ChatId).HasColumnName("chat_id");
            builder.Property(e => e.UserId).HasColumnName("user_id");
            builder.Property(e => e.CanAddMembers)
                .HasDefaultValue(false)
                .HasColumnName("can_add_members");
            builder.Property(e => e.CanPost)
                .HasDefaultValue(true)
                .HasColumnName("can_post");
            builder.Property(e => e.CanRemoveMembers)
                .HasDefaultValue(false)
                .HasColumnName("can_remove_members");
            builder.Property(e => e.JoinedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("joined_at");
            builder.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValueSql("'member'::character varying")
                .HasColumnName("role");

            builder.HasOne(d => d.Chat).WithMany(p => p.ChatMembers)
                .HasForeignKey(d => d.ChatId)
                .HasConstraintName("chat_members_chat_id_fkey");

            builder.HasOne(d => d.User).WithMany(p => p.ChatMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("chat_members_user_id_fkey");
        }
    }
}
