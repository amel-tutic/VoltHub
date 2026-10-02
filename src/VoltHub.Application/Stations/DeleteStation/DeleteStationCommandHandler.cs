using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Stations.DeleteStation;

internal sealed class DeleteStationCommandHandler(IApplicationDbContext db) : IRequestHandler<DeleteStationCommand, Result>
{
    public async Task<Result> Handle(DeleteStationCommand request, CancellationToken cancellationToken)
    {
        var station = await db.ChargingStations.SingleOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (station is null)
            return StationErrors.NotFound;

        // The database would refuse anyway (Restrict); checking first gives a clear message instead of an error.
        if (await db.Chargers.AnyAsync(c => c.StationId == station.Id, cancellationToken))
            return StationErrors.HasChargers;

        db.ChargingStations.Remove(station);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
