using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonoVik.WebApi.UserBlocks.Infrastructure
{
    public sealed class UserBlockConfiguration : IEntityTypeConfiguration<UserBlock>
    {
        public void Configure(EntityTypeBuilder<UserBlock> builder)
        {
            builder.HasKey(e => new { e.BlockerId, e.BlockedId }).HasName("user_blocks_pkey");

            builder.ToTable("user_blocks");

            builder.HasIndex(e => e.BlockerId, "idx_user_blocks_blocker_id");

            builder.Property(e => e.BlockerId).HasColumnName("blocker_id");
            builder.Property(e => e.BlockedId).HasColumnName("blocked_id");
            builder.Property(e => e.BlockedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("blocked_at");

            builder.HasOne(d => d.Blocked).WithMany(p => p.UserBlockBlockeds)
                .HasForeignKey(d => d.BlockedId)
                .HasConstraintName("user_blocks_blocked_id_fkey");

            builder.HasOne(d => d.Blocker).WithMany(p => p.UserBlockBlockers)
                .HasForeignKey(d => d.BlockerId)
                .HasConstraintName("user_blocks_blocker_id_fkey");
        }
    }
}
