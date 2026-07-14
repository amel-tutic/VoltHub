using VoltHub.Domain.Common;

namespace VoltHub.Domain.Entities;

public sealed class Rating : BaseEntity
{
    public int Score { get; private set; }
    public string? Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid StationId { get; private set; }
    public Guid UserId { get; private set; }

    private Rating() { }

    private Rating(Guid id, int score, string? comment, Guid stationId, Guid userId)
    {
        Id = id;
        Score = score;
        Comment = comment;
        StationId = stationId;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    // One-rating-per-(user,station) is a cross-row uniqueness rule — enforced by a DB
    // UNIQUE(station_id, user_id) constraint in the EF configuration, not checkable here.
    public static Rating Create(int score, Guid stationId, Guid userId, string? comment = null)
    {
        if (score is < 1 or > 5)
            throw new ArgumentOutOfRangeException(nameof(score), "Score must be between 1 and 5.");

        return new Rating(Guid.CreateVersion7(), score, comment?.Trim(), stationId, userId);
    }
}