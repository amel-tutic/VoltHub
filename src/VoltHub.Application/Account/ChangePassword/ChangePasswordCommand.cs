using MediatR;
using VoltHub.Application.Auth;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Account.ChangePassword;

// Returns a fresh token pair: every other device is signed out, but this one keeps working.
public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest<Result<AuthResponse>>;
