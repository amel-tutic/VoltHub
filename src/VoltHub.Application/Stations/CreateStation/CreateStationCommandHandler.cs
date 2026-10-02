using MediatR;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Entities;

namespace VoltHub.Application.Stations.CreateStation;

internal sealed class CreateStationCommandHandler(IApplicationDbContext db) : IRequestHandler<CreateStationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateStationCommand request, CancellationToken cancellationToken)
    {
        var station = ChargingStation.Create(
            request.Name, request.Address, request.City, request.Latitude, request.Longitude, request.Description);

        db.ChargingStations.Add(station);
        await db.SaveChangesAsync(cancellationToken);
        return station.Id;
    }
}
