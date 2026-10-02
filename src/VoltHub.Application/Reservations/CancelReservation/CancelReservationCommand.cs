using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Reservations.CancelReservation;

public sealed record CancelReservationCommand(Guid Id) : IRequest<Result>;
