using Microsoft.EntityFrameworkCore;
using VoltHub.Domain.Entities;

namespace VoltHub.Application.Common.Interfaces;

// What use cases need from the database. Implemented by VoltHubDbContext in Infrastructure.
public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Vehicle> Vehicles { get; }
    DbSet<ChargingStation> ChargingStations { get; }
    DbSet<Charger> Chargers { get; }
    DbSet<Reservation> Reservations { get; }
    DbSet<ChargingSession> ChargingSessions { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<Payment> Payments { get; }
    DbSet<MaintenanceRecord> MaintenanceRecords { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<Rating> Ratings { get; }
    DbSet<ProblemReport> ProblemReports { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
