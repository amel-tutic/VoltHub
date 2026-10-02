using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Stations;

internal static class StationErrors
{
    public static readonly Error NotFound = Error.NotFound("Station.NotFound", "Charging station not found.");
    public static readonly Error HasChargers = Error.Conflict("Station.HasChargers", "Remove the station's chargers before deleting it.");
}
