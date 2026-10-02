using MediatR;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Entities;

namespace VoltHub.Application.Vehicles.AddVehicle;

internal sealed class AddVehicleCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<AddVehicleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddVehicleCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        // The owner always comes from the token, never from the request body.
        var vehicle = Vehicle.Create(request.Make, request.Model, request.BatteryCapacityKwh, request.ConnectorType, userId);
        db.Vehicles.Add(vehicle);
        await db.SaveChangesAsync(cancellationToken);
        return vehicle.Id;
    }
}
