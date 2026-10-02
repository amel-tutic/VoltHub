using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Vehicles;

internal static class VehicleErrors
{
    // Someone else's vehicle is reported as "not found", so ids of other users' data are never confirmed.
    public static readonly Error NotFound = Error.NotFound("Vehicle.NotFound", "Vehicle not found.");
    public static readonly Error HasReservations = Error.Conflict("Vehicle.HasReservations",
        "A vehicle with reservations can't be deleted, so that its charging history is preserved.");
}
