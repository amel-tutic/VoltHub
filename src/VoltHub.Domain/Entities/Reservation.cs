using VoltHub.Domain.Common;
using VoltHub.Domain.Enums;

namespace VoltHub.Domain.Entities;

public sealed class Reservation : BaseEntity
{
    // A "book now" request reaches the server a moment after it was made; tolerate that much.
    private static readonly TimeSpan StartGracePeriod = TimeSpan.FromMinutes(5);

    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid VehicleId { get; private set; }
    public Vehicle Vehicle { get; private set; } = default!;
    public Guid ChargerId { get; private set; }
    public Charger Charger { get; private set; } = default!;

    private Reservation() { }

    private Reservation(Guid id, DateTime startTime, DateTime endTime, Guid vehicleId, Guid chargerId)
    {
        Id = id;
        StartTime = startTime;
        EndTime = endTime;
        Status = ReservationStatus.Active;
        VehicleId = vehicleId;
        ChargerId = chargerId;
        CreatedAt = DateTime.UtcNow;
    }

    // The reserving user is reached through the vehicle (Vehicle.UserId); storing it here too
    // would duplicate that relationship. Only rules a single Reservation can check about itself
    // live here — "no overlapping Active slots per charger and per vehicle" compares rows, so it
    // lives in the Application handler and the PostgreSQL exclusion constraints.
    public static Reservation Create(DateTime startTime, DateTime endTime, Guid vehicleId, Guid chargerId)
    {
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time.", nameof(endTime));
        if (startTime < DateTime.UtcNow - StartGracePeriod)
            throw new ArgumentException("Start time cannot be in the past.", nameof(startTime));

        return new Reservation(Guid.CreateVersion7(), startTime, endTime, vehicleId, chargerId);
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
