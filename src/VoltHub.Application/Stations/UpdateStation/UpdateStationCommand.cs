using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Stations.UpdateStation;

public sealed record UpdateStationCommand(
    Guid Id, string Name, string Address, string City, double Latitude, double Longitude, string? Description)
    : IRequest<Result>;
