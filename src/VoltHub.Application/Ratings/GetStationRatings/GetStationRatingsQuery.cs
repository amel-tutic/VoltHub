using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Ratings.GetStationRatings;

public sealed record GetStationRatingsQuery(Guid StationId) : IRequest<Result<StationRatingsResponse>>;
