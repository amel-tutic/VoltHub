using MediatR;
using VoltHub.Application.Auth.GetMe;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Account.UpdateProfile;

// SSA process 1.3: any role edits its own name. Email and role are not editable here.
public sealed record UpdateProfileCommand(string FirstName, string LastName) : IRequest<Result<UserProfileResponse>>;
