using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Stations.GetNearestStations;

internal sealed class GetNearestStationsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetNearestStationsQuery, Result<List<NearestStationResponse>>>
{
    public async Task<Result<List<NearestStationResponse>>> Handle(GetNearestStationsQuery request, CancellationToken cancellationToken)
    {
        var available = ChargerAvailability.AvailableNow(db, DateTime.UtcNow);
        if (request.ConnectorType is { } connector)
            available = available.Where(c => c.ConnectorType == connector);

        // Filter in the database; measure distances in memory (a charging network has few stations).
        var candidates = await db.ChargingStations
            .Where(s => available.Any(c => c.StationId == s.Id))
            .Select(s => new { s.Id, s.Name, s.Address, s.City, s.Latitude, s.Longitude,
                               Available = available.Count(c => c.StationId == s.Id) })
            .ToListAsync(cancellationToken);

        return candidates
            .Select(s => new NearestStationResponse(s.Id, s.Name, s.Address, s.City, s.Latitude, s.Longitude, s.Available,
                Math.Round(Geo.DistanceKm(request.Latitude, request.Longitude, s.Latitude, s.Longitude), 2)))
            .OrderBy(s => s.DistanceKm)
            .Take(request.Take)
            .ToList();
    }
}
