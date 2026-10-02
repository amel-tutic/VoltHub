using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Chargers.SetChargerPrice;

public sealed record SetChargerPriceCommand(Guid ChargerId, decimal PricePerKwh) : IRequest<Result>;
