using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Chargers.SetChargerStatus;

internal sealed class SetChargerStatusCommandHandler(IApplicationDbContext db) : IRequestHandler<SetChargerStatusCommand, Result>
{
    public async Task<Result> Handle(SetChargerStatusCommand request, CancellationToken cancellationToken)
    {
        var charger = await db.Chargers.SingleOrDefaultAsync(c => c.Id == request.ChargerId, cancellationToken);
        if (charger is null)
            return ChargerErrors.NotFound;

        charger.SetStatus(request.Status);   // also records StatusChangedAt, used by the "offline too long" rule
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
