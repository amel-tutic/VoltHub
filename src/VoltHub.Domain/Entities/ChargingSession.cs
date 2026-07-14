using VoltHub.Domain.Common;
using VoltHub.Domain.Enums;

namespace VoltHub.Domain.Entities;

public sealed class ChargingSession : BaseEntity
{
    public DateTime StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public decimal? EnergyKwh { get; private set; }
    public decimal PricePerKwh { get; private set; }
    public decimal? TotalPrice { get; private set; }
    public SessionStatus Status { get; private set; }
    public Guid UserId { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid ChargerId { get; private set; }
    public Guid? ReservationId { get; private set; }

    private ChargingSession() { }

    private ChargingSession(Guid id, decimal pricePerKwh, Guid userId, Guid vehicleId, Guid chargerId, Guid? reservationId)
    {
        Id = id;
        StartedAt = DateTime.UtcNow;
        PricePerKwh = pricePerKwh;
        Status = SessionStatus.InProgress;
        UserId = userId;
        VehicleId = vehicleId;
        ChargerId = chargerId;
        ReservationId = reservationId;
    }

    // pricePerKwh is passed in, not read from a Charger reference: rule #3 ("price snapshot")
    // requires the price valid *at charging time* to be frozen onto the session, immune to any
    // later change on the charger. Session never holds a live reference to Charger or Vehicle —
    // it receives exactly the data it needs, which also keeps it trivially unit-testable.
    public static ChargingSession Create(decimal pricePerKwh, Guid userId, Guid vehicleId, Guid chargerId, Guid? reservationId = null)
    {
        if (pricePerKwh < 0)
            throw new ArgumentOutOfRangeException(nameof(pricePerKwh), "Price cannot be negative.");

        return new ChargingSession(Guid.CreateVersion7(), pricePerKwh, userId, vehicleId, chargerId, reservationId);
    }

    // Rule #4 (energy simulation): energy is capped by whichever limit binds first — what the
    // charger could deliver in the elapsed time, or the vehicle's remaining battery capacity.
    public void Complete(decimal chargerPowerKw, decimal vehicleBatteryCapacityKwh)
    {
        if (Status != SessionStatus.InProgress)
            throw new InvalidOperationException($"Cannot complete a session with status '{Status}'.");

        EndedAt = DateTime.UtcNow;
        var hours = (decimal)(EndedAt.Value - StartedAt).TotalHours;
        var deliverable = chargerPowerKw * hours * 0.9m; // 0.9 = charging efficiency factor
        EnergyKwh = Math.Round(Math.Min(deliverable, vehicleBatteryCapacityKwh), 3);
        TotalPrice = Math.Round(EnergyKwh.Value * PricePerKwh, 2);
        Status = SessionStatus.Completed;
    }
}