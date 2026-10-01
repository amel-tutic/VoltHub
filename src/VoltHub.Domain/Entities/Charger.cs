using VoltHub.Domain.Common;
using VoltHub.Domain.Enums;

namespace VoltHub.Domain.Entities;

// Service dates are not stored here: the last and next service dates are derived from this
// charger's MaintenanceRecords, so there is a single source of truth for maintenance data.
public sealed class Charger : BaseEntity
{
    public string Code { get; private set; } = default!;
    public ConnectorType ConnectorType { get; private set; }
    public CurrentType CurrentType { get; private set; }
    public decimal PowerKw { get; private set; }
    public decimal PricePerKwh { get; private set; }
    public ChargerStatus Status { get; private set; }
    public DateTime StatusChangedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid StationId { get; private set; }
    public ChargingStation Station { get; private set; } = default!;

    private Charger() { }

    private Charger(Guid id, string code, ConnectorType connectorType, CurrentType currentType, decimal powerKw, decimal pricePerKwh, Guid stationId)
    {
        Id = id;
        Code = code;
        ConnectorType = connectorType;
        CurrentType = currentType;
        PowerKw = powerKw;
        PricePerKwh = pricePerKwh;
        Status = ChargerStatus.Available;
        StationId = stationId;
        CreatedAt = DateTime.UtcNow;
        StatusChangedAt = CreatedAt;
    }

    public static Charger Create(string code, ConnectorType connectorType, CurrentType currentType, decimal powerKw, decimal pricePerKwh, Guid stationId)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.", nameof(code));
        if (powerKw <= 0)
            throw new ArgumentOutOfRangeException(nameof(powerKw), "Power must be greater than zero.");
        if (pricePerKwh < 0)
            throw new ArgumentOutOfRangeException(nameof(pricePerKwh), "Price cannot be negative.");

        return new Charger(Guid.CreateVersion7(), code.Trim(), connectorType, currentType, powerKw, pricePerKwh, stationId);
    }

    public void SetStatus(ChargerStatus status)
    {
        if (status is not (ChargerStatus.Available or ChargerStatus.OutOfOrder or ChargerStatus.UnderMaintenance))
            throw new ArgumentException(
                $"Operators cannot set status to '{status}' directly; Occupied/Reserved are derived from active sessions/reservations.",
                nameof(status));

        // Re-submitting the current status must not restart the clock that the
        // "out of operation for too long" notification rule measures from.
        if (status == Status)
            return;

        Status = status;
        StatusChangedAt = DateTime.UtcNow;
    }

    public void SetPrice(decimal pricePerKwh)
    {
        if (pricePerKwh < 0)
            throw new ArgumentOutOfRangeException(nameof(pricePerKwh), "Price cannot be negative.");

        PricePerKwh = pricePerKwh;
    }
}
