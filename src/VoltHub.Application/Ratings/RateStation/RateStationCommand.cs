using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Ratings.RateStation;

// SSA process 8.3: one rating per user per station. Rating again updates the existing one.
public sealed record RateStationCommand(Guid StationId, int Score, string? Comment) : IRequest<Result>;
