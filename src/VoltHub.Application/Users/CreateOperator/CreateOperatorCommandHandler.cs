using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Entities;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Users.CreateOperator;

internal sealed class CreateOperatorCommandHandler(IApplicationDbContext db, IPasswordHasher passwordHasher)
    : IRequestHandler<CreateOperatorCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOperatorCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Email == email, cancellationToken))
            return UserErrors.EmailTaken;

        var user = User.Create(request.FirstName, request.LastName, email, passwordHasher.Hash(request.Password), UserRole.Operator);
        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);
        return user.Id;
    }
}
