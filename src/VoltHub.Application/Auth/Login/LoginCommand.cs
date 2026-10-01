using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;
