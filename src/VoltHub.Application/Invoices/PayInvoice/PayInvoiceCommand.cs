using MediatR;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Invoices.PayInvoice;

public sealed record PayInvoiceCommand(Guid InvoiceId, PaymentMethod Method) : IRequest<Result<PaymentResponse>>;
