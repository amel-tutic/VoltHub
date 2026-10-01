using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Auth.GetMe;

public sealed record GetMeQuery : IRequest<Result<UserProfileResponse>>;

public sealed record UserProfileResponse(Guid Id, string FirstName, string LastName, string Email, string Role);
