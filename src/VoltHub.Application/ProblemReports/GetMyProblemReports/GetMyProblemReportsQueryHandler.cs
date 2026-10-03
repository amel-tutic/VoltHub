using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.ProblemReports.GetMyProblemReports;

internal sealed class GetMyProblemReportsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMyProblemReportsQuery, Result<List<ProblemReportResponse>>>
{
    public async Task<Result<List<ProblemReportResponse>>> Handle(GetMyProblemReportsQuery request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        return await db.ProblemReports
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(ProblemReportProjection.ToResponse)
            .ToListAsync(cancellationToken);
    }
}
