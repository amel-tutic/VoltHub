using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoltHub.Domain.Entities;

namespace VoltHub.Infrastructure.Persistence.Configurations;

internal sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Make).HasMaxLength(100);
        builder.Property(v => v.Model).HasMaxLength(100);
        builder.Property(v => v.BatteryCapacityKwh).HasPrecision(6, 2);
        builder.Property(v => v.ConnectorType).HasConversion<string>().HasMaxLength(20);

        builder.ToTable(t => t.HasCheckConstraint("ck_vehicles_battery_capacity_positive", "battery_capacity_kwh > 0"));

        builder.HasOne(v => v.User).WithMany().HasForeignKey(v => v.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
