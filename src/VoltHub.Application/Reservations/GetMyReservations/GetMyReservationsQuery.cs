using MediatR;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Reservations.GetMyReservations;

public sealed record GetMyReservationsQuery(ReservationStatus? Status) : IRequest<Result<List<ReservationResponse>>>;
