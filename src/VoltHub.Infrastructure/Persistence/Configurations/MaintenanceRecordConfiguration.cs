using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoltHub.Domain.Entities;

namespace VoltHub.Infrastructure.Persistence.Configurations;

internal sealed class MaintenanceRecordConfiguration : IEntityTypeConfiguration<MaintenanceRecord>
{
    public void Configure(EntityTypeBuilder<MaintenanceRecord> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.Description).HasMaxLength(1000);

        builder.HasOne(m => m.Charger).WithMany().HasForeignKey(m => m.ChargerId).OnDelete(DeleteBehavior.Restrict);
    }
}
