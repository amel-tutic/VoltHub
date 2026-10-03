using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Maintenance.ResolveMaintenance;

// Closes one open record: a fault that was fixed, or a planned service that was carried out.
public sealed record ResolveMaintenanceCommand(Guid RecordId) : IRequest<Result>;
