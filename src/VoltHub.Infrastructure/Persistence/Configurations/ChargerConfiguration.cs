using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoltHub.Domain.Entities;

namespace VoltHub.Infrastructure.Persistence.Configurations;

internal sealed class ChargerConfiguration : IEntityTypeConfiguration<Charger>
{
    public void Configure(EntityTypeBuilder<Charger> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code).HasMaxLength(50);
        builder.Property(c => c.ConnectorType).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.CurrentType).HasConversion<string>().HasMaxLength(5);
        builder.Property(c => c.PowerKw).HasPrecision(6, 2);
        builder.Property(c => c.PricePerKwh).HasPrecision(8, 2);
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(30);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_chargers_power_positive", "power_kw > 0");
            t.HasCheckConstraint("ck_chargers_price_non_negative", "price_per_kwh >= 0");
        });

        // Code is unique within a station, not globally ("A1" can exist at every station).
        builder.HasIndex(c => new { c.StationId, c.Code }).IsUnique();

        builder.HasOne(c => c.Station).WithMany().HasForeignKey(c => c.StationId).OnDelete(DeleteBehavior.Restrict);
    }
}
