using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Reports.GetRevenueReport;

internal sealed class GetRevenueReportQueryHandler(IApplicationDbContext db) : IRequestHandler<GetRevenueReportQuery, Result<RevenueReportResponse>>
{
    public async Task<Result<RevenueReportResponse>> Handle(GetRevenueReportQuery request, CancellationToken cancellationToken)
    {
        var (from, to) = ReportPeriod.Resolve(request.From, request.To);

        var sessions = await db.ChargingSessions
            .Where(s => s.Status == SessionStatus.Completed && s.EndedAt >= from && s.EndedAt < to)
            .Select(s => new { EndedAt = s.EndedAt!.Value, Energy = s.EnergyKwh ?? 0m, Revenue = s.TotalPrice ?? 0m })
            .ToListAsync(cancellationToken);

        var invoices = await db.Invoices
            .Where(i => i.IssuedAt >= from && i.IssuedAt < to && i.Status != InvoiceStatus.Cancelled)
            .Select(i => new { i.Amount, i.Status })
            .ToListAsync(cancellationToken);

        var paid = await db.Payments
            .Where(p => p.Status == PaymentStatus.Completed && p.PaidAt >= from && p.PaidAt < to)
            .Select(p => p.Amount)
            .ToListAsync(cancellationToken);

        var byDay = sessions
            .GroupBy(s => DateOnly.FromDateTime(s.EndedAt))
            .OrderBy(g => g.Key)
            .Select(g => new RevenueByDayResponse(g.Key, g.Count(), g.Sum(s => s.Energy), g.Sum(s => s.Revenue)))
            .ToList();

        return new RevenueReportResponse(
            from, to,
            sessions.Count,
            sessions.Sum(s => s.Energy),
            Billed: invoices.Sum(i => i.Amount),
            Paid: paid.Sum(),
            Outstanding: invoices.Where(i => i.Status == InvoiceStatus.Pending).Sum(i => i.Amount),
            byDay);
    }
}
