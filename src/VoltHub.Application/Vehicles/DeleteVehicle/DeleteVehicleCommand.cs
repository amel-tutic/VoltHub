using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Vehicles.DeleteVehicle;

public sealed record DeleteVehicleCommand(Guid Id) : IRequest<Result>;
