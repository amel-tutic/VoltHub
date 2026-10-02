using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VoltHub.Domain.Enums;
using VoltHub.Infrastructure.Persistence;

namespace VoltHub.Infrastructure.BackgroundJobs;

// SSA process 3.4: once a reserved slot has ended without a charging session, the reservation expires.
// Runs inside the API process, once a minute.
internal sealed class ReservationExpiryService(IServiceScopeFactory scopeFactory, ILogger<ReservationExpiryService> logger)
    : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);
        do
        {
            try
            {
                await ExpireUnusedReservationsAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Reservation expiry run failed; retrying on the next tick");   // never kill the loop
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task ExpireUnusedReservationsAsync(CancellationToken cancellationToken)
    {
        // A background service lives for the whole app, but a DbContext must be short-lived: one scope per run.
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VoltHubDbContext>();
        var now = DateTime.UtcNow;

        var unused = await db.Reservations
            .Where(r => r.Status == ReservationStatus.Active && r.EndTime <= now &&
                        !db.ChargingSessions.Any(s => s.ReservationId == r.Id))
            .ToListAsync(cancellationToken);
        if (unused.Count == 0)
            return;

        foreach (var reservation in unused)
            reservation.Expire();

        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Expired {Count} unused reservation(s)", unused.Count);
    }
}
