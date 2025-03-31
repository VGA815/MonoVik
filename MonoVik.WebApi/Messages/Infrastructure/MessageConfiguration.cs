using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonoVik.WebApi.Messages.Infrastructure
{
    public sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(e => e.MessageId).HasName("messages_pkey");

            builder.ToTable("messages");

            builder.HasIndex(e => new { e.ChatId, e.CreatedAt }, "idx_messages_chat_id_created_at");

            builder.Property(e => e.MessageId)
                .ValueGeneratedNever()
                .HasColumnName("message_id");
            builder.Property(e => e.AttachmentUrl).HasColumnName("attachment_url");
            builder.Property(e => e.ChatId).HasColumnName("chat_id");
            builder.Property(e => e.Content).HasColumnName("content");
            builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            builder.Property(e => e.IsEdited)
                .HasDefaultValue(false)
                .HasColumnName("is_edited");
            builder.Property(e => e.SenderId).HasColumnName("sender_id");
            builder.Property(e => e.Tags).HasColumnName("tags");
            builder.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            builder.HasOne(d => d.Chat).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("messages_chat_id_fkey");

            builder.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("messages_sender_id_fkey");
        }
    }
}
