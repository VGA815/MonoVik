using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonoVik.WebApi.MediaFiles.Infrastructure
{
    public sealed class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
    {
        public void Configure(EntityTypeBuilder<MediaFile> builder)
        {
            builder.HasKey(e => e.MediaId).HasName("media_files_pkey");

            builder.ToTable("media_files");

            builder.Property(e => e.MediaId)
                .ValueGeneratedNever()
                .HasColumnName("media_id");
            builder.Property(e => e.FileSize).HasColumnName("file_size");
            builder.Property(e => e.FileType)
                .HasMaxLength(50)
                .HasColumnName("file_type");
            builder.Property(e => e.FileUrl).HasColumnName("file_url");
            builder.Property(e => e.IsPublic)
                .HasDefaultValue(false)
                .HasColumnName("is_public");
            builder.Property(e => e.UploadDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("upload_date");
            builder.Property(e => e.UploaderId).HasColumnName("uploader_id");

            builder.HasOne(d => d.Uploader).WithMany(p => p.MediaFiles)
                .HasForeignKey(d => d.UploaderId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("media_files_uploader_id_fkey");
        }
    }
}
