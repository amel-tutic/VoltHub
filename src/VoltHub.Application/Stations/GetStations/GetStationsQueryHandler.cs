using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Stations.GetStations;

internal sealed class GetStationsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetStationsQuery, Result<List<StationSummaryResponse>>>
{
    public async Task<Result<List<StationSummaryResponse>>> Handle(GetStationsQuery request, CancellationToken cancellationToken)
    {
        var available = ChargerAvailability.AvailableNow(db, DateTime.UtcNow);
        var stations = db.ChargingStations.AsQueryable();

        // Each filter narrows the query; nothing runs in the database until ToListAsync.
        if (!string.IsNullOrWhiteSpace(request.City))
        {
            var city = request.City.Trim().ToLower();
            stations = stations.Where(s => s.City.ToLower() == city);
        }
        if (request.ConnectorType is { } connector)
            stations = stations.Where(s => db.Chargers.Any(c => c.StationId == s.Id && c.ConnectorType == connector));
        if (request.CurrentType is { } current)
            stations = stations.Where(s => db.Chargers.Any(c => c.StationId == s.Id && c.CurrentType == current));
        if (request.MinPowerKw is { } minPower)
            stations = stations.Where(s => db.Chargers.Any(c => c.StationId == s.Id && c.PowerKw >= minPower));
        if (request.OnlyAvailable)
            stations = stations.Where(s => available.Any(c => c.StationId == s.Id));

        return await stations
            .OrderBy(s => s.City).ThenBy(s => s.Name)
            .Select(s => new StationSummaryResponse(
                s.Id, s.Name, s.Address, s.City, s.Latitude, s.Longitude,
                db.Chargers.Count(c => c.StationId == s.Id),
                available.Count(c => c.StationId == s.Id),
                db.Ratings.Where(r => r.StationId == s.Id).Average(r => (double?)r.Score)))
            .ToListAsync(cancellationToken);
    }
}
