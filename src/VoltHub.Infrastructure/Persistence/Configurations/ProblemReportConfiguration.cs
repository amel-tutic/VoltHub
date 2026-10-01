using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoltHub.Domain.Entities;

namespace VoltHub.Infrastructure.Persistence.Configurations;

internal sealed class ProblemReportConfiguration : IEntityTypeConfiguration<ProblemReport>
{
    public void Configure(EntityTypeBuilder<ProblemReport> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Type).HasConversion<string>().HasMaxLength(30);
        builder.Property(p => p.Description).HasMaxLength(1000);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(p => p.Charger).WithMany().HasForeignKey(p => p.ChargerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
