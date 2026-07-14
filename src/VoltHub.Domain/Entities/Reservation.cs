using VoltHub.Domain.Common;
using VoltHub.Domain.Enums;

namespace VoltHub.Domain.Entities;

public sealed class Reservation : BaseEntity
{
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid UserId { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid ChargerId { get; private set; }

    private Reservation() { }

    private Reservation(Guid id, DateTime startTime, DateTime endTime, Guid userId, Guid vehicleId, Guid chargerId)
    {
        Id = id;
        StartTime = startTime;
        EndTime = endTime;
        Status = ReservationStatus.Active;
        UserId = userId;
        VehicleId = vehicleId;
        ChargerId = chargerId;
        CreatedAt = DateTime.UtcNow;
    }

    // Only enforces what a single Reservation can validate about itself. The other half of
    // business rule #1 — no two Active reservations may overlap on the same charger — needs
    // to compare against other rows, so it lives in the Application-layer handler and the
    // PostgreSQL exclusion constraint (S4), not here.
    public static Reservation Create(DateTime startTime, DateTime endTime, Guid userId, Guid vehicleId, Guid chargerId)
    {
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time.", nameof(endTime));
        if (startTime < DateTime.UtcNow)
            throw new ArgumentException("Start time cannot be in the past.", nameof(startTime));

        return new Reservation(Guid.CreateVersion7(), startTime, endTime, userId, vehicleId, chargerId);
    }

    public void Cancel()
    {
        if (Status != ReservationStatus.Active)
            throw new InvalidOperationException($"Cannot cancel a reservation with status '{Status}'.");
        Status = ReservationStatus.Cancelled;
    }

    public void Complete()
    {
        if (Status != ReservationStatus.Active)
            throw new InvalidOperationException($"Cannot complete a reservation with status '{Status}'.");
        Status = ReservationStatus.Completed;
    }

    public void Expire()
    {
        if (Status != ReservationStatus.Active)
            throw new InvalidOperationException($"Cannot expire a reservation with status '{Status}'.");
        Status = ReservationStatus.Expired;
    }
}