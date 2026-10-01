using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoltHub.Domain.Entities;

namespace VoltHub.Infrastructure.Persistence.Configurations;

internal sealed class RatingConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        // Composite key: the M:N "ocenjuje" relationship's identity is the (user, station) pair.
        builder.HasKey(r => new { r.UserId, r.StationId });

        builder.Property(r => r.Comment).HasMaxLength(1000);

        builder.ToTable(t => t.HasCheckConstraint("ck_ratings_score_range", "score BETWEEN 1 AND 5"));

        builder.HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(r => r.Station).WithMany().HasForeignKey(r => r.StationId).OnDelete(DeleteBehavior.Cascade);
    }
}
