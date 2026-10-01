using Microsoft.EntityFrameworkCore;
using VoltHub.Domain.Entities;

namespace VoltHub.Infrastructure.Persistence;

public sealed class VoltHubDbContext(DbContextOptions<VoltHubDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<ChargingStation> ChargingStations => Set<ChargingStation>();
    public DbSet<Charger> Chargers => Set<Charger>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<ChargingSession> ChargingSessions => Set<ChargingSession>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<MaintenanceRecord> MaintenanceRecords => Set<MaintenanceRecord>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<ProblemReport> ProblemReports => Set<ProblemReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(VoltHubDbContext).Assembly);
}
