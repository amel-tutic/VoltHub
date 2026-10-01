using VoltHub.Domain.Common;
using VoltHub.Domain.Enums;

namespace VoltHub.Domain.Entities;

public sealed class ChargingSession : BaseEntity
{
    private const decimal ChargingEfficiency = 0.9m;

    public DateTime StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public decimal? EnergyKwh { get; private set; }
    public decimal PricePerKwh { get; private set; }
    public decimal? TotalPrice { get; private set; }
    public SessionStatus Status { get; private set; }
    public Guid ReservationId { get; private set; }
    public Reservation Reservation { get; private set; } = default!;

    private ChargingSession() { }

    private ChargingSession(Guid id, decimal pricePerKwh, Guid reservationId)
    {
        Id = id;
        StartedAt = DateTime.UtcNow;
        PricePerKwh = pricePerKwh;
        Status = SessionStatus.InProgress;
        ReservationId = reservationId;
    }

    // Every session realizes exactly one reservation; its user, vehicle and charger are reached
    // through that reservation instead of being stored again. pricePerKwh is still passed in:
    // rule #3 freezes the price valid at charging time onto the session.
    public static ChargingSession Create(decimal pricePerKwh, Guid reservationId)
    {
        if (pricePerKwh < 0)
            throw new ArgumentOutOfRangeException(nameof(pricePerKwh), "Price cannot be negative.");

        return new ChargingSession(Guid.CreateVersion7(), pricePerKwh, reservationId);
    }

    // Live view of an active session: same formula as Complete(), evaluated without changing state.
    public decimal EstimateEnergyKwh(DateTime asOfUtc, decimal chargerPowerKw, decimal vehicleBatteryCapacityKwh)
    {
        if (Status != SessionStatus.InProgress)
            throw new InvalidOperationException($"Cannot estimate a session with status '{Status}'.");

        return CalculateEnergyKwh(asOfUtc - StartedAt, chargerPowerKw, vehicleBatteryCapacityKwh);
    }

    public void Complete(decimal chargerPowerKw, decimal vehicleBatteryCapacityKwh)
    {
        if (Status != SessionStatus.InProgress)
            throw new InvalidOperationException($"Cannot complete a session with status '{Status}'.");

        EndedAt = DateTime.UtcNow;
        EnergyKwh = CalculateEnergyKwh(EndedAt.Value - StartedAt, chargerPowerKw, vehicleBatteryCapacityKwh);
        TotalPrice = Math.Round(EnergyKwh.Value * PricePerKwh, 2);
        Status = SessionStatus.Completed;
    }

    // Rule #4: energy is capped by whichever limit binds first — what the charger can deliver in
    // the elapsed time, or the vehicle's battery capacity. One formula shared by both callers.
    private static decimal CalculateEnergyKwh(TimeSpan elapsed, decimal chargerPowerKw, decimal vehicleBatteryCapacityKwh)
    {
        var hours = (decimal)Math.Max(elapsed.TotalHours, 0);
        var deliverable = chargerPowerKw * hours * ChargingEfficiency;
        return Math.Round(Math.Min(deliverable, vehicleBatteryCapacityKwh), 3);
    }
}
