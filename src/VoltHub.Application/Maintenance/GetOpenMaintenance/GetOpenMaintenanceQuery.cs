using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Maintenance.GetOpenMaintenance;

// The operator's to-do list: open faults and services still planned, across the whole network.
public sealed record GetOpenMaintenanceQuery : IRequest<Result<List<OpenMaintenanceItemResponse>>>;
