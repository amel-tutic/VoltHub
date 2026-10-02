using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Stations.CreateStation;

public sealed record CreateStationCommand(
    string Name, string Address, string City, double Latitude, double Longitude, string? Description)
    : IRequest<Result<Guid>>;
