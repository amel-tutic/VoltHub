using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Entities;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Sessions.StopCharging;

// SSA processes 4.3 + 5.1: finish the session, price it, complete the reservation, issue the invoice.
internal sealed class StopChargingCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<StopChargingCommand, Result<SessionSummaryResponse>>
{
    public async Task<Result<SessionSummaryResponse>> Handle(StopChargingCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        var session = await db.ChargingSessions
            .Include(s => s.Reservation).ThenInclude(r => r.Charger)
            .Include(s => s.Reservation).ThenInclude(r => r.Vehicle)
            .SingleOrDefaultAsync(s => s.Id == request.SessionId && s.Reservation.Vehicle.UserId == userId, cancellationToken);
        if (session is null)
            return SessionErrors.NotFound;
        if (session.Status != SessionStatus.InProgress)
            return SessionErrors.NotInProgress;

        var reservation = session.Reservation;
        session.Complete(reservation.Charger.PowerKw, reservation.Vehicle.BatteryCapacityKwh);
        reservation.Complete();

        var totalPrice = session.TotalPrice!.Value;
        var invoice = Invoice.Create(NewInvoiceNumber(session.Id), totalPrice, session.Id);
        db.Invoices.Add(invoice);

        // Session, reservation and invoice are saved in ONE transaction: all three change, or none does.
        await db.SaveChangesAsync(cancellationToken);

        return new SessionSummaryResponse(
            session.Id, session.StartedAt, session.EndedAt!.Value, session.EnergyKwh!.Value,
            session.PricePerKwh, totalPrice, invoice.Id, invoice.InvoiceNumber);
    }

    // Readable and unique: issue date + the random tail of the session's UUIDv7 (a unique index guards the rest).
    private static string NewInvoiceNumber(Guid sessionId) =>
        $"VH-{DateTime.UtcNow:yyyyMMdd}-{sessionId.ToString("N")[^8..].ToUpperInvariant()}";
}
