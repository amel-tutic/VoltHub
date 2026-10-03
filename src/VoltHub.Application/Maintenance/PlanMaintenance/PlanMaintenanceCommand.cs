using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Maintenance.PlanMaintenance;

// SSA process 7.3: a regular service planned for a future date.
public sealed record PlanMaintenanceCommand(Guid ChargerId, DateTimeOffset ScheduledDate, string Description) : IRequest<Result<Guid>>;
