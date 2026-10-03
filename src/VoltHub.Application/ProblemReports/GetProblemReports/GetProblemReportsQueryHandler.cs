using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Entities;

namespace VoltHub.Application.ProblemReports.GetProblemReports;

internal sealed class GetProblemReportsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetProblemReportsQuery, Result<List<ProblemReportResponse>>>
{
    public async Task<Result<List<ProblemReportResponse>>> Handle(GetProblemReportsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<ProblemReport> query = db.ProblemReports;
        if (request.Status is { } status)
            query = query.Where(p => p.Status == status);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(ProblemReportProjection.ToResponse)
            .ToListAsync(cancellationToken);
    }
}
