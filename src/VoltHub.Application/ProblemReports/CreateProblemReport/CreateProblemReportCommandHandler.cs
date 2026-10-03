using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Entities;

namespace VoltHub.Application.ProblemReports.CreateProblemReport;

internal sealed class CreateProblemReportCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<CreateProblemReportCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProblemReportCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;
        if (!await db.Chargers.AnyAsync(c => c.Id == request.ChargerId, cancellationToken))
            return ProblemReportErrors.ChargerNotFound;

        var report = ProblemReport.Create(request.Type, request.Description, request.ChargerId, userId);
        db.ProblemReports.Add(report);
        await db.SaveChangesAsync(cancellationToken);   // operators are notified by the notification job (SSA 7.4)
        return report.Id;
    }
}
