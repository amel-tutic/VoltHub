using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Auth.GetMe;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Account.UpdateProfile;

internal sealed class UpdateProfileCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<UpdateProfileCommand, Result<UserProfileResponse>>
{
    public async Task<Result<UserProfileResponse>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        var user = await db.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
            return AccountErrors.UserNotFound;

        user.UpdateProfile(request.FirstName, request.LastName);
        await db.SaveChangesAsync(cancellationToken);

        return new UserProfileResponse(user.Id, user.FirstName, user.LastName, user.Email, user.Role.ToString());
    }
}
