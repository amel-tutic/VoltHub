using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Reservations.GetChargerSchedule;

// Taken slots of one charger in a window (default: the next 7 days) — what a booking calendar needs.
public sealed record GetChargerScheduleQuery(Guid ChargerId, DateTimeOffset? From, DateTimeOffset? To)
    : IRequest<Result<List<BusySlotResponse>>>;
