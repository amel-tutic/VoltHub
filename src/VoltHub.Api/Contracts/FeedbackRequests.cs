using VoltHub.Domain.Enums;

namespace VoltHub.Api.Contracts;

public sealed record RatingRequest(int Score, string? Comment);
public sealed record ProblemStatusRequest(ProblemStatus Status);
