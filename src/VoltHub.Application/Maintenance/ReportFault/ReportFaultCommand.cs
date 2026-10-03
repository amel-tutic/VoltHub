using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Maintenance.ReportFault;

// SSA process 7.1: a fault is recorded and the charger goes out of order, so nobody can book it.
public sealed record ReportFaultCommand(Guid ChargerId, string Description) : IRequest<Result<Guid>>;
