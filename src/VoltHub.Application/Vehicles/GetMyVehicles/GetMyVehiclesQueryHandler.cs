using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Vehicles.GetMyVehicles;

internal sealed class GetMyVehiclesQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMyVehiclesQuery, Result<List<VehicleResponse>>>
{
    public async Task<Result<List<VehicleResponse>>> Handle(GetMyVehiclesQuery request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        return await db.Vehicles
            .Where(v => v.UserId == userId)
            .OrderBy(v => v.CreatedAt)
            .Select(v => new VehicleResponse(v.Id, v.Make, v.Model, v.BatteryCapacityKwh, v.ConnectorType))
            .ToListAsync(cancellationToken);
    }
}
