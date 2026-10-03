using System.Linq.Expressions;
using VoltHub.Domain.Entities;

namespace VoltHub.Application.ProblemReports;

// One projection shared by the owner's and the operator's lists, translated to SQL by EF Core.
internal static class ProblemReportProjection
{
    public static readonly Expression<Func<ProblemReport, ProblemReportResponse>> ToResponse = p => new ProblemReportResponse(
        p.Id, p.Type, p.Description, p.Status, p.CreatedAt, p.ResolvedAt,
        p.ChargerId, p.Charger.Code, p.Charger.StationId, p.Charger.Station.Name,
        p.User.FirstName + " " + p.User.LastName);
}
