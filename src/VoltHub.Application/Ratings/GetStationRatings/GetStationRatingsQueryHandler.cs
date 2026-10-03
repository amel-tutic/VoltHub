using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Ratings.GetStationRatings;

internal sealed class GetStationRatingsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetStationRatingsQuery, Result<StationRatingsResponse>>
{
    public async Task<Result<StationRatingsResponse>> Handle(GetStationRatingsQuery request, CancellationToken cancellationToken)
    {
        if (!await db.ChargingStations.AnyAsync(s => s.Id == request.StationId, cancellationToken))
            return RatingErrors.StationNotFound;

        var ratings = await db.Ratings
            .Where(r => r.StationId == request.StationId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new
            {
                r.UserId,
                Item = new RatingResponse(r.User.FirstName + " " + r.User.LastName.Substring(0, 1) + ".", r.Score, r.Comment, r.CreatedAt)
            })
            .ToListAsync(cancellationToken);

        var mine = ratings.SingleOrDefault(r => r.UserId == currentUser.UserId)?.Item;
        return new StationRatingsResponse(
            ratings.Count == 0 ? null : Math.Round(ratings.Average(r => r.Item.Score), 1),
            ratings.Count,
            mine is null ? null : new MyRatingResponse(mine.Score, mine.Comment),
            ratings.Select(r => r.Item).ToList());
    }
}
