using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Auth.Register;

public sealed record RegisterCommand(string FirstName, string LastName, string Email, string Password)
    : IRequest<Result<AuthResponse>>;
