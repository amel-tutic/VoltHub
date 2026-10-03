using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Maintenance.GetChargerMaintenance;

// SSA process 7.5: a charger's full maintenance history.
public sealed record GetChargerMaintenanceQuery(Guid ChargerId) : IRequest<Result<ChargerMaintenanceResponse>>;
