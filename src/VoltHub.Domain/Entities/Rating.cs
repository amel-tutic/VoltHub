namespace VoltHub.Domain.Entities;

// Maps the M:N "rates" relationship between User and ChargingStation. Its identity IS the
// (UserId, StationId) pair — a composite primary key — so "at most one rating per user per
// station" is guaranteed by the key itself. That is why it has no surrogate Id / BaseEntity.
public sealed class Rating
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;
    public Guid StationId { get; private set; }
    public ChargingStation Station { get; private set; } = default!;
    public int Score { get; private set; }
    public string? Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Rating() { }

    private Rating(Guid userId, Guid stationId, int score, string? comment)
    {
        UserId = userId;
        StationId = stationId;
        Score = score;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
    }

    public static Rating Create(Guid userId, Guid stationId, int score, string? comment = null)
    {
        EnsureValidScore(score);
        return new Rating(userId, stationId, score, comment?.Trim());
    }

    // Re-rating a station updates the existing row instead of adding a second one.
    public void Update(int score, string? comment)
    {
        EnsureValidScore(score);
        Score = score;
        Comment = comment?.Trim();
    }

    private static void EnsureValidScore(int score)
    {
        if (score is < 1 or > 5)
            throw new ArgumentOutOfRangeException(nameof(score), "Score must be between 1 and 5.");
    }
}
