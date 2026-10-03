namespace VoltHub.Application.Ratings;

public sealed record RatingResponse(string Author, int Score, string? Comment, DateTime CreatedAt);

public sealed record MyRatingResponse(int Score, string? Comment);

public sealed record StationRatingsResponse(double? Average, int Count, MyRatingResponse? Mine, IReadOnlyList<RatingResponse> Items);
