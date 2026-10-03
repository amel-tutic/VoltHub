using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Entities;

namespace VoltHub.Application.Ratings.RateStation;

internal sealed class RateStationCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<RateStationCommand, Result>
{
    public async Task<Result> Handle(RateStationCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;
        if (!await db.ChargingStations.AnyAsync(s => s.Id == request.StationId, cancellationToken))
            return RatingErrors.StationNotFound;

        var comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment;

        // The (user, station) pair is the primary key, so an existing rating is updated, never duplicated.
        var existing = await db.Ratings
            .SingleOrDefaultAsync(r => r.UserId == userId && r.StationId == request.StationId, cancellationToken);
        if (existing is null)
            db.Ratings.Add(Rating.Create(userId, request.StationId, request.Score, comment));
        else
            existing.Update(request.Score, comment);

        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
