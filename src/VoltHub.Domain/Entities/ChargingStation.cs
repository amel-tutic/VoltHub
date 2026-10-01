using VoltHub.Domain.Common;

namespace VoltHub.Domain.Entities;

public sealed class ChargingStation : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string Address { get; private set; } = default!;
    public string City { get; private set; } = default!;
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ChargingStation() { }

    private ChargingStation(Guid id, string name, string address, string city, double latitude, double longitude, string? description)
    {
        Id = id;
        Name = name;
        Address = address;
        City = city;
        Latitude = latitude;
        Longitude = longitude;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    public static ChargingStation Create(string name, string address, string city, double latitude, double longitude, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address is required.", nameof(address));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required.", nameof(city));
        if (latitude is < -90 or > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");
        if (longitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");

        return new ChargingStation(Guid.CreateVersion7(), name.Trim(), address.Trim(), city.Trim(), latitude, longitude, description?.Trim());
    }

    public void UpdateDetails(string name, string address, string city, double latitude, double longitude, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address is required.", nameof(address));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required.", nameof(city));
        if (latitude is < -90 or > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");
        if (longitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");

        Name = name.Trim();
        Address = address.Trim();
        City = city.Trim();
        Latitude = latitude;
        Longitude = longitude;
        Description = description?.Trim();
    }
}

