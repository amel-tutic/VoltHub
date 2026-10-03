using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Domain.Entities;
using VoltHub.Domain.Enums;

namespace VoltHub.Infrastructure.Notifications;

// SSA process 7.4: once a minute, turns three situations into notifications for every active operator:
//   - a planned service is due soon             (reads D9, writes D10)
//   - a charger has been out of order too long  (reads D4, writes D10)
//   - a new problem report arrived              (reads D12, writes D10)
// Each situation is notified once: the job first checks whether that notification already exists.
internal sealed class NotificationJob(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<NotificationJob> logger)
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
                await RunAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Notification run failed; retrying on the next tick");   // never kill the loop
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var now = DateTime.UtcNow;

        var operators = await db.Users
            .Where(u => u.Role == UserRole.Operator && u.IsActive)
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);
        if (operators.Count == 0)
            return;

        // Thresholds can be shortened in appsettings.Development.json to demo the rules live.
        var dueWithin = TimeSpan.FromHours(configuration.GetValue("Notifications:ServiceDueWithinHours", 24.0));
        var offlineAfter = TimeSpan.FromHours(configuration.GetValue("Notifications:OfflineAfterHours", 48.0));

        var created = await NotifyServicesDueAsync(db, operators, now + dueWithin, cancellationToken)
                    + await NotifyChargersOfflineAsync(db, operators, now - offlineAfter, cancellationToken)
                    + await NotifyNewProblemReportsAsync(db, operators, now, cancellationToken);

        if (created > 0)
        {
            await db.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Created notifications for {Count} new situation(s)", created);
        }
    }

    private static async Task<int> NotifyServicesDueAsync(
        IApplicationDbContext db, List<Guid> operators, DateTime dueBy, CancellationToken cancellationToken)
    {
        var due = await db.MaintenanceRecords
            .Where(m => m.Type == MaintenanceType.ScheduledService && m.ResolvedAt == null &&
                        m.ScheduledDate != null && m.ScheduledDate <= dueBy)
            .Select(m => new { m.ScheduledDate, m.Description, m.Charger.Code, StationName = m.Charger.Station.Name })
            .ToListAsync(cancellationToken);

        var count = 0;
        foreach (var item in due)
        {
            var title = Fit($"Service due: {item.StationName} {item.Code}", 150);
            var message = Fit($"Planned for {item.ScheduledDate:dd.MM.yyyy HH:mm} UTC: {item.Description}", 500);
            if (await db.Notifications.AnyAsync(n =>
                    n.Type == NotificationType.MaintenanceDue && n.Title == title && n.Message == message, cancellationToken))
                continue;

            Notify(db, operators, NotificationType.MaintenanceDue, title, message);
            count++;
        }
        return count;
    }

    private static async Task<int> NotifyChargersOfflineAsync(
        IApplicationDbContext db, List<Guid> operators, DateTime offlineSince, CancellationToken cancellationToken)
    {
        var offline = await db.Chargers
            .Where(c => c.Status == ChargerStatus.OutOfOrder && c.StatusChangedAt <= offlineSince)
            .Select(c => new { c.Code, c.StatusChangedAt, StationName = c.Station.Name })
            .ToListAsync(cancellationToken);

        var count = 0;
        foreach (var charger in offline)
        {
            var title = Fit($"Charger offline: {charger.StationName} {charger.Code}", 150);
            // Once per outage: skip if this charger was already reported after the outage began.
            if (await db.Notifications.AnyAsync(n =>
                    n.Type == NotificationType.ChargerOffline && n.Title == title && n.CreatedAt >= charger.StatusChangedAt, cancellationToken))
                continue;

            var message = $"Out of order since {charger.StatusChangedAt:dd.MM.yyyy HH:mm} UTC. Repair it or plan a service.";
            Notify(db, operators, NotificationType.ChargerOffline, title, message);
            count++;
        }
        return count;
    }

    private static async Task<int> NotifyNewProblemReportsAsync(
        IApplicationDbContext db, List<Guid> operators, DateTime now, CancellationToken cancellationToken)
    {
        var recent = now.AddDays(-7);
        var reports = await db.ProblemReports
            .Where(p => p.Status == ProblemStatus.Open && p.CreatedAt >= recent)
            .Select(p => new { p.Id, p.Type, p.Description, p.Charger.Code, StationName = p.Charger.Station.Name })
            .ToListAsync(cancellationToken);

        var count = 0;
        foreach (var report in reports)
        {
            // UUIDv7 ids start with a timestamp, so the random tail is what tells reports apart.
            var reference = $"[ref {report.Id.ToString("N")[^8..]}]";
            if (await db.Notifications.AnyAsync(n =>
                    n.Type == NotificationType.NewProblemReport && n.Message.EndsWith(reference), cancellationToken))
                continue;

            var title = Fit($"New problem report: {report.StationName} {report.Code}", 150);
            var message = Fit($"{report.Type}: {report.Description}", 500 - reference.Length - 1) + " " + reference;
            Notify(db, operators, NotificationType.NewProblemReport, title, message);
            count++;
        }
        return count;
    }

    private static void Notify(IApplicationDbContext db, List<Guid> operators, NotificationType type, string title, string message)
    {
        foreach (var operatorId in operators)
            db.Notifications.Add(Notification.Create(type, title, message, operatorId));
    }

    private static string Fit(string text, int maxLength) =>
        text.Length <= maxLength ? text : text[..(maxLength - 1)] + "…";
}
