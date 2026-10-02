using VoltHub.Domain.Entities;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Stations;

public sealed record StationSummaryResponse(
    Guid Id, string Name, string Address, string City, double Latitude, double Longitude,
    int ChargerCount, int AvailableChargerCount, double? AverageRating);

public sealed record StationDetailsResponse(
    Guid Id, string Name, string Address, string City, double Latitude, double Longitude, string? Description,
    double? AverageRating, int RatingCount, IReadOnlyList<ChargerResponse> Chargers);

public sealed record NearestStationResponse(
    Guid Id, string Name, string Address, string City, double Latitude, double Longitude,
    int AvailableChargers, double DistanceKm);

public sealed record ChargerResponse(
    Guid Id, string Code, ConnectorType ConnectorType, CurrentType CurrentType,
    decimal PowerKw, decimal PricePerKwh, ChargerStatus Status)
{
    public static ChargerResponse From(Charger charger, ChargerStatus effectiveStatus) =>
        new(charger.Id, charger.Code, charger.ConnectorType, charger.CurrentType, charger.PowerKw, charger.PricePerKwh, effectiveStatus);
}
