using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Invoices.GetMyInvoices;

internal sealed class GetMyInvoicesQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMyInvoicesQuery, Result<List<InvoiceResponse>>>
{
    public async Task<Result<List<InvoiceResponse>>> Handle(GetMyInvoicesQuery request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        return await db.Invoices
            .Where(i => i.Session.Reservation.Vehicle.UserId == userId)
            .OrderByDescending(i => i.IssuedAt)
            .Select(i => new InvoiceResponse(
                i.Id, i.InvoiceNumber, i.Amount, i.IssuedAt, i.Status, i.SessionId,
                i.Session.Reservation.Charger.Station.Name,
                db.Payments.Where(p => p.InvoiceId == i.Id && p.Status == PaymentStatus.Completed).Max(p => p.PaidAt)))
            .ToListAsync(cancellationToken);
    }
}
