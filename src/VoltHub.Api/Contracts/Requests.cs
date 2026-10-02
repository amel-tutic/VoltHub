using VoltHub.Domain.Enums;

namespace VoltHub.Api.Contracts;

// HTTP request bodies. The resource id comes from the URL, so it isn't repeated in the body.
public sealed record StationRequest(string Name, string Address, string City, double Latitude, double Longitude, string? Description);
public sealed record ChargerRequest(string Code, ConnectorType ConnectorType, CurrentType CurrentType, decimal PowerKw, decimal PricePerKwh);
public sealed record ChargerPriceRequest(decimal PricePerKwh);
public sealed record ChargerStatusRequest(ChargerStatus Status);

public sealed record CreatedResponse(Guid Id);
