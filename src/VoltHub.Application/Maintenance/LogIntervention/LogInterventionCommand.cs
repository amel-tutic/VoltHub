using MediatR;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Maintenance.LogIntervention;

// SSA process 7.2: work done on the charger right now (a repair or an inspection).
// A repair also closes every open fault and puts the charger back into service.
public sealed record LogInterventionCommand(Guid ChargerId, MaintenanceType Type, string Description) : IRequest<Result<Guid>>;
