using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoltHub.Domain.Entities;

namespace VoltHub.Infrastructure.Persistence.Configurations;

// The two "no overlapping Active slots" rules (per charger, per vehicle) are PostgreSQL exclusion
// constraints added in the AddReservationOverlapConstraints migration: EF Core has no API for them.
internal sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);

        builder.ToTable(t => t.HasCheckConstraint("ck_reservations_end_after_start", "end_time > start_time"));

        builder.HasOne(r => r.Vehicle).WithMany().HasForeignKey(r => r.VehicleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(r => r.Charger).WithMany().HasForeignKey(r => r.ChargerId).OnDelete(DeleteBehavior.Restrict);
    }
}
