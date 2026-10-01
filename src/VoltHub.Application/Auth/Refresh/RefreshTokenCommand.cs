using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Auth.Refresh;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponse>>;
