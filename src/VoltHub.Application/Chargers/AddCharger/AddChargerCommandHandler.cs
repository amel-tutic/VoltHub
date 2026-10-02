using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Application.Stations;
using VoltHub.Domain.Entities;

namespace VoltHub.Application.Chargers.AddCharger;

internal sealed class AddChargerCommandHandler(IApplicationDbContext db) : IRequestHandler<AddChargerCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddChargerCommand request, CancellationToken cancellationToken)
    {
        if (!await db.ChargingStations.AnyAsync(s => s.Id == request.StationId, cancellationToken))
            return StationErrors.NotFound;

        var code = request.Code.Trim();
        if (await db.Chargers.AnyAsync(c => c.StationId == request.StationId && c.Code == code, cancellationToken))
            return ChargerErrors.CodeTaken(code);

        var charger = Charger.Create(code, request.ConnectorType, request.CurrentType, request.PowerKw, request.PricePerKwh, request.StationId);
        db.Chargers.Add(charger);
        await db.SaveChangesAsync(cancellationToken);
        return charger.Id;
    }
}
