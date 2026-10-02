using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Stations.UpdateStation;

internal sealed class UpdateStationCommandHandler(IApplicationDbContext db) : IRequestHandler<UpdateStationCommand, Result>
{
    public async Task<Result> Handle(UpdateStationCommand request, CancellationToken cancellationToken)
    {
        var station = await db.ChargingStations.SingleOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (station is null)
            return StationErrors.NotFound;

        station.UpdateDetails(request.Name, request.Address, request.City, request.Latitude, request.Longitude, request.Description);
        await db.SaveChangesAsync(cancellationToken);   // EF noticed the changed properties and updates only those
        return Result.Success();
    }
}
