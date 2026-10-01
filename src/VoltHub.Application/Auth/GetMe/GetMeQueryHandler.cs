using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Auth.GetMe;

internal sealed class GetMeQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMeQuery, Result<UserProfileResponse>>
{
    public async Task<Result<UserProfileResponse>> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return Error.Unauthorized("Auth.NotAuthenticated", "You are not logged in.");

        var profile = await db.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserProfileResponse(u.Id, u.FirstName, u.LastName, u.Email, u.Role.ToString()))
            .SingleOrDefaultAsync(cancellationToken);

        if (profile is null)
            return Error.NotFound("User.NotFound", "User not found.");

        return profile;
    }
}
