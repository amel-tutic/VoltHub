using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Entities;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Invoices.PayInvoice;

// SSA process 5.3: pay an invoice through the external payment provider.
internal sealed class PayInvoiceCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IPaymentGateway paymentGateway)
    : IRequestHandler<PayInvoiceCommand, Result<PaymentResponse>>
{
    public async Task<Result<PaymentResponse>> Handle(PayInvoiceCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        var invoice = await db.Invoices.SingleOrDefaultAsync(
            i => i.Id == request.InvoiceId && i.Session.Reservation.Vehicle.UserId == userId, cancellationToken);
        if (invoice is null)
            return InvoiceErrors.NotFound;
        if (invoice.Status != InvoiceStatus.Pending)
            return InvoiceErrors.NotPayable(invoice.Status);

        var payment = Payment.Create(request.Method, invoice.Amount, invoice.Id);
        db.Payments.Add(payment);

        var approved = await paymentGateway.ChargeAsync(invoice.Amount, request.Method, invoice.InvoiceNumber, cancellationToken);
        if (approved)
        {
            payment.Complete();
            invoice.MarkPaid();
        }
        else
        {
            payment.Fail();
        }

        await db.SaveChangesAsync(cancellationToken);   // failed attempts are recorded too (Invoice 1 : N Payment)

        if (!approved)
            return InvoiceErrors.PaymentDeclined;

        return new PaymentResponse(payment.Id, invoice.Id, invoice.InvoiceNumber, payment.Amount, payment.Method, payment.Status, payment.PaidAt);
    }
}
