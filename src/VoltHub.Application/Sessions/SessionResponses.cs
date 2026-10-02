using VoltHub.Domain.Enums;

namespace VoltHub.Application.Sessions;

public sealed record ActiveSessionResponse(
    Guid SessionId, Guid ReservationId, string StationName, string ChargerCode, string VehicleName,
    DateTime StartedAt, double ElapsedMinutes, decimal EstimatedEnergyKwh, decimal PricePerKwh, decimal EstimatedCost,
    DateTime ReservationEndsAt);

public sealed record SessionSummaryResponse(
    Guid SessionId, DateTime StartedAt, DateTime EndedAt, decimal EnergyKwh, decimal PricePerKwh, decimal TotalPrice,
    Guid InvoiceId, string InvoiceNumber);

public sealed record SessionHistoryResponse(
    Guid SessionId, DateTime StartedAt, DateTime? EndedAt, SessionStatus Status,
    string StationName, string ChargerCode, string VehicleName, decimal? EnergyKwh, decimal? TotalPrice);
