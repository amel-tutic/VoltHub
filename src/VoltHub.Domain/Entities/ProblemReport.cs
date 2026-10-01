using VoltHub.Domain.Common;
using VoltHub.Domain.Enums;

namespace VoltHub.Domain.Entities;

public sealed class ProblemReport : BaseEntity
{
    public ProblemType Type { get; private set; }
    public string Description { get; private set; } = default!;
    public ProblemStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public Guid ChargerId { get; private set; }
    public Charger Charger { get; private set; } = default!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    private ProblemReport() { }

    private ProblemReport(Guid id, ProblemType type, string description, Guid chargerId, Guid userId)
    {
        Id = id;
        Type = type;
        Description = description;
        Status = ProblemStatus.Open;
        ChargerId = chargerId;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    // Every report concerns a specific charger; the station is reached through it.
    public static ProblemReport Create(ProblemType type, string description, Guid chargerId, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.", nameof(description));

        return new ProblemReport(Guid.CreateVersion7(), type, description.Trim(), chargerId, userId);
    }

    public void MarkInProgress()
    {
        if (Status != ProblemStatus.Open)
            throw new InvalidOperationException($"Cannot mark in-progress a report with status '{Status}'.");
        Status = ProblemStatus.InProgress;
    }

    public void Resolve()
    {
        if (Status == ProblemStatus.Resolved)
            throw new InvalidOperationException("This report is already resolved.");
        Status = ProblemStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;
    }
}
