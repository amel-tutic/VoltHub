using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Chargers.SetChargerPrice;

// Running sessions keep the price they started with (ChargingSession stores its own PricePerKwh).
internal sealed class SetChargerPriceCommandHandler(IApplicationDbContext db) : IRequestHandler<SetChargerPriceCommand, Result>
{
    public async Task<Result> Handle(SetChargerPriceCommand request, CancellationToken cancellationToken)
    {
        var charger = await db.Chargers.SingleOrDefaultAsync(c => c.Id == request.ChargerId, cancellationToken);
        if (charger is null)
            return ChargerErrors.NotFound;

        charger.SetPrice(request.PricePerKwh);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
