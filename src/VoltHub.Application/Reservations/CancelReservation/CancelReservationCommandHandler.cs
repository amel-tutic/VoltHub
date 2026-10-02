using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Reservations.CancelReservation;

internal sealed class CancelReservationCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<CancelReservationCommand, Result>
{
    public async Task<Result> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        // Ownership goes through the vehicle: a reservation belongs to whoever owns its vehicle.
        var reservation = await db.Reservations
            .SingleOrDefaultAsync(r => r.Id == request.Id && r.Vehicle.UserId == userId, cancellationToken);
        if (reservation is null)
            return ReservationErrors.NotFound;

        if (reservation.Status != ReservationStatus.Active)
            return ReservationErrors.NotActive(reservation.Status);
        if (reservation.StartTime <= DateTime.UtcNow)
            return ReservationErrors.AlreadyStarted;

        reservation.Cancel();
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
