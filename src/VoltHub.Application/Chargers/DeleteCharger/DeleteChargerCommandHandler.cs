using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Chargers.DeleteCharger;

internal sealed class DeleteChargerCommandHandler(IApplicationDbContext db) : IRequestHandler<DeleteChargerCommand, Result>
{
    public async Task<Result> Handle(DeleteChargerCommand request, CancellationToken cancellationToken)
    {
        var charger = await db.Chargers.SingleOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (charger is null)
            return ChargerErrors.NotFound;

        // Reservations (and through them sessions and invoices), maintenance records and problem reports
        // all point at the charger with Restrict. Their history must stay, so a used charger is kept;
        // the operator sets it out of order instead. Checking first gives a clear message, not a database error.
        var hasHistory =
            await db.Reservations.AnyAsync(r => r.ChargerId == charger.Id, cancellationToken)
            || await db.MaintenanceRecords.AnyAsync(m => m.ChargerId == charger.Id, cancellationToken)
            || await db.ProblemReports.AnyAsync(p => p.ChargerId == charger.Id, cancellationToken);
        if (hasHistory)
            return ChargerErrors.HasHistory;

        db.Chargers.Remove(charger);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
