using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonoVik.WebApi.Chats.Infrastructure
{
    public sealed class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.HasKey(e => e.ChatId).HasName("chats_pkey");

            builder.ToTable("chats");

            builder.Property(e => e.ChatId)
                .ValueGeneratedNever()
                .HasColumnName("chat_id");
            builder.Property(e => e.ChatDescription).HasColumnName("chat_description");
            builder.Property(e => e.ChatImageUrl).HasColumnName("chat_image_url");
            builder.Property(e => e.ChatName)
                .HasMaxLength(100)
                .HasColumnName("chat_name");
            builder.Property(e => e.ChatType)
                .HasMaxLength(20)
                .HasDefaultValueSql("'private'::character varying")
                .HasColumnName("chat_type");
            builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            builder.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            builder.Property(e => e.IsGroup)
                .HasDefaultValue(false)
                .HasColumnName("is_group");
            builder.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");
        }
    }
}
