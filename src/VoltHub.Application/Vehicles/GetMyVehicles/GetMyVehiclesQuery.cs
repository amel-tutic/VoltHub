using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Vehicles.GetMyVehicles;

public sealed record GetMyVehiclesQuery : IRequest<Result<List<VehicleResponse>>>;
