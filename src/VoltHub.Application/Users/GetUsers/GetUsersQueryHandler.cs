using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Entities;

namespace VoltHub.Application.Users.GetUsers;

internal sealed class GetUsersQueryHandler(IApplicationDbContext db) : IRequestHandler<GetUsersQuery, Result<List<UserAdminResponse>>>
{
    public async Task<Result<List<UserAdminResponse>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        IQueryable<User> query = db.Users;
        if (request.Role is { } role)
            query = query.Where(u => u.Role == role);
        if (request.IsActive is { } isActive)
            query = query.Where(u => u.IsActive == isActive);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(u => u.Email.Contains(term) || u.FirstName.ToLower().Contains(term) || u.LastName.ToLower().Contains(term));
        }

        return await query
            .OrderBy(u => u.Role).ThenBy(u => u.LastName).ThenBy(u => u.FirstName)
            .Select(u => new UserAdminResponse(
                u.Id, u.FirstName, u.LastName, u.Email, u.Role, u.IsActive, u.CreatedAt,
                db.Vehicles.Count(v => v.UserId == u.Id)))
            .ToListAsync(cancellationToken);
    }
}
