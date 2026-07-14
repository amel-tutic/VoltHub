using VoltHub.Domain.Common;
using VoltHub.Domain.Enums;

namespace VoltHub.Domain.Entities;

public sealed class MaintenanceRecord : BaseEntity
{
    public MaintenanceType Type { get; private set; }
    public string Description { get; private set; } = default!;
    public DateTime ReportedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public DateTime? ScheduledDate { get; private set; }
    public Guid ChargerId { get; private set; }

    private MaintenanceRecord() { }

    private MaintenanceRecord(Guid id, MaintenanceType type, string description, DateTime? scheduledDate, Guid chargerId)
    {
        Id = id;
        Type = type;
        Description = description;
        ReportedAt = DateTime.UtcNow;
        ScheduledDate = scheduledDate;
        ChargerId = chargerId;
    }

    public static MaintenanceRecord Create(MaintenanceType type, string description, Guid chargerId, DateTime? scheduledDate = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.", nameof(description));

        return new MaintenanceRecord(Guid.CreateVersion7(), type, description.Trim(), scheduledDate, chargerId);
    }

    public void Resolve()
    {
        if (ResolvedAt is not null)
            throw new InvalidOperationException("This record is already resolved.");
        ResolvedAt = DateTime.UtcNow;
    }
}