using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Vehicles.UpdateVehicle;

internal sealed class UpdateVehicleCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<UpdateVehicleCommand, Result>
{
    public async Task<Result> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        var vehicle = await db.Vehicles.SingleOrDefaultAsync(v => v.Id == request.Id && v.UserId == userId, cancellationToken);
        if (vehicle is null)
            return VehicleErrors.NotFound;

        vehicle.UpdateDetails(request.Make, request.Model, request.BatteryCapacityKwh, request.ConnectorType);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
