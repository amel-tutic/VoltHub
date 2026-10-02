using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Stations.DeleteStation;

public sealed record DeleteStationCommand(Guid Id) : IRequest<Result>;
