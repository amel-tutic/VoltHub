using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Vehicles.DeleteVehicle;

internal sealed class DeleteVehicleCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<DeleteVehicleCommand, Result>
{
    public async Task<Result> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        var vehicle = await db.Vehicles.SingleOrDefaultAsync(v => v.Id == request.Id && v.UserId == userId, cancellationToken);
        if (vehicle is null)
            return VehicleErrors.NotFound;

        if (await db.Reservations.AnyAsync(r => r.VehicleId == vehicle.Id, cancellationToken))
            return VehicleErrors.HasReservations;

        db.Vehicles.Remove(vehicle);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
