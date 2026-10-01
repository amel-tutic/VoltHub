using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoltHub.Domain.Entities;

namespace VoltHub.Infrastructure.Persistence.Configurations;

internal sealed class ChargingStationConfiguration : IEntityTypeConfiguration<ChargingStation>
{
    public void Configure(EntityTypeBuilder<ChargingStation> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).HasMaxLength(150);
        builder.Property(s => s.Address).HasMaxLength(255);
        builder.Property(s => s.City).HasMaxLength(100);
        builder.Property(s => s.Description).HasMaxLength(1000);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_charging_stations_latitude_range", "latitude BETWEEN -90 AND 90");
            t.HasCheckConstraint("ck_charging_stations_longitude_range", "longitude BETWEEN -180 AND 180");
        });

        builder.HasIndex(s => s.City);
    }
}
