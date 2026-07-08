using VoltHub.Domain.Common;
using VoltHub.Domain.Enums;

namespace VoltHub.Domain.Entities;

public sealed class Vehicle : BaseEntity
{
    public string Make { get; private set; } = default!;
    public string Model { get; private set; } = default!;
    public decimal BatteryCapacityKwh { get; private set; }
    public ConnectorType ConnectorType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid UserId { get; private set; }

    private Vehicle() { }

    private Vehicle(Guid id, string make, string model, decimal batteryCapacityKwh, ConnectorType connectorType, Guid userId)
    {
        Id = id;
        Make = make;
        Model = model;
        BatteryCapacityKwh = batteryCapacityKwh;
        ConnectorType = connectorType;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public static Vehicle Create(string make, string model, decimal batteryCapacityKwh, ConnectorType connectorType, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(make))
            throw new ArgumentException("Make is required.", nameof(make));
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model is required.", nameof(model));
        if (batteryCapacityKwh <= 0)
            throw new ArgumentOutOfRangeException(nameof(batteryCapacityKwh), "Battery capacity must be greater than zero.");

        return new Vehicle(Guid.NewGuid(), make.Trim(), model.Trim(), batteryCapacityKwh, connectorType, userId);
    }

    public void UpdateDetails(string make, string model, decimal batteryCapacityKwh, ConnectorType connectorType)
    {
        if (string.IsNullOrWhiteSpace(make))
            throw new ArgumentException("Make is required.", nameof(make));
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model is required.", nameof(model));
        if (batteryCapacityKwh <= 0)
            throw new ArgumentOutOfRangeException(nameof(batteryCapacityKwh), "Battery capacity must be greater than zero.");

        Make = make.Trim();
        Model = model.Trim();
        BatteryCapacityKwh = batteryCapacityKwh;
        ConnectorType = connectorType;
    }
}