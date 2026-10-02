using VoltHub.Domain.Enums;

namespace VoltHub.Application.Reservations;

public sealed record ReservationResponse(
    Guid Id, DateTime StartTime, DateTime EndTime, ReservationStatus Status,
    Guid ChargerId, string ChargerCode, Guid StationId, string StationName,
    Guid VehicleId, string VehicleName);

// A taken slot on a charger's calendar — deliberately without any data about who booked it.
public sealed record BusySlotResponse(DateTime StartTime, DateTime EndTime);
