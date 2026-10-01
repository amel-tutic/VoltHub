using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoltHub.Domain.Entities;

namespace VoltHub.Infrastructure.Persistence.Configurations;

internal sealed class ChargingSessionConfiguration : IEntityTypeConfiguration<ChargingSession>
{
    public void Configure(EntityTypeBuilder<ChargingSession> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.EnergyKwh).HasPrecision(8, 3);
        builder.Property(s => s.PricePerKwh).HasPrecision(8, 2);
        builder.Property(s => s.TotalPrice).HasPrecision(10, 2);
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_charging_sessions_energy_non_negative", "energy_kwh >= 0");
            t.HasCheckConstraint("ck_charging_sessions_price_non_negative", "price_per_kwh >= 0");
            t.HasCheckConstraint("ck_charging_sessions_total_non_negative", "total_price >= 0");
        });

        // 1:1 — each reservation is realized by at most one session (EF adds the unique index).
        builder.HasOne(s => s.Reservation).WithOne().HasForeignKey<ChargingSession>(s => s.ReservationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
