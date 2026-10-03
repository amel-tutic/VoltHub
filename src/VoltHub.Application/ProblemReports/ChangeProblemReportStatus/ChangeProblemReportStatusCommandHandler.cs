using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.ProblemReports.ChangeProblemReportStatus;

internal sealed class ChangeProblemReportStatusCommandHandler(IApplicationDbContext db)
    : IRequestHandler<ChangeProblemReportStatusCommand, Result>
{
    public async Task<Result> Handle(ChangeProblemReportStatusCommand request, CancellationToken cancellationToken)
    {
        var report = await db.ProblemReports.SingleOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (report is null)
            return ProblemReportErrors.NotFound;

        // Check the transition here so a wrong request gets a clear 409 instead of a domain exception.
        switch (request.Status)
        {
            case ProblemStatus.InProgress when report.Status == ProblemStatus.Open:
                report.MarkInProgress();
                break;
            case ProblemStatus.Resolved when report.Status != ProblemStatus.Resolved:
                report.Resolve();
                break;
            default:
                return ProblemReportErrors.InvalidTransition(report.Status, request.Status);
        }

        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
