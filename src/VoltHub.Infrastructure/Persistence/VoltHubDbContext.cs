using Microsoft.EntityFrameworkCore;
using Npgsql;
using VoltHub.Application.Common.Exceptions;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Domain.Entities;

namespace VoltHub.Infrastructure.Persistence;

public sealed class VoltHubDbContext(DbContextOptions<VoltHubDbContext> options) : DbContext(options), IApplicationDbContext
{
    // Readable messages for constraints the API can hit when two requests race each other.
    private static readonly Dictionary<string, string> ConflictMessages = new()
    {
        ["ix_users_email"] = "An account with this email already exists.",
        ["ex_reservations_charger_overlap"] = "The charger is already reserved for an overlapping time slot.",
        ["ex_reservations_vehicle_overlap"] = "This vehicle already has a reservation in an overlapping time slot."
    };

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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation or PostgresErrorCodes.ExclusionViolation
        } postgres)
        {
            // Translate a PostgreSQL-specific error into an application-level one (→ 409 Conflict).
            var message = postgres.ConstraintName is not null && ConflictMessages.TryGetValue(postgres.ConstraintName, out var friendly)
                ? friendly
                : "The operation conflicts with existing data.";
            throw new ConflictException(message, ex);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(VoltHubDbContext).Assembly);
}
