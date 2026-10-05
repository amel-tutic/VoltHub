using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Chargers.DeleteCharger;

// SSA 6.2: remove a charger that was added by mistake. Only a charger with no history can go.
public sealed record DeleteChargerCommand(Guid Id) : IRequest<Result>;
