using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Entities;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Auth.Register;

internal sealed class RegisterCommandHandler(IApplicationDbContext db, IPasswordHasher passwordHasher, ITokenService tokens)
    : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Email == email, cancellationToken))
            return Error.Conflict("Auth.EmailTaken", "An account with this email already exists.");

        // Self-registration always creates an EV owner; staff accounts are created by the administrator.
        var user = User.Create(request.FirstName, request.LastName, email, passwordHasher.Hash(request.Password), UserRole.User);
        db.Users.Add(user);

        var response = TokenIssuer.Issue(user, tokens, db);
        await db.SaveChangesAsync(cancellationToken);
        return response;
    }
}
