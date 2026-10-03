using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Ratings;

internal static class RatingErrors
{
    public static readonly Error StationNotFound = Error.NotFound("Rating.StationNotFound", "Charging station not found.");
}
