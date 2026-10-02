using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Invoices;

internal static class InvoiceErrors
{
    public static readonly Error NotFound = Error.NotFound("Invoice.NotFound", "Invoice not found.");
    public static readonly Error PaymentDeclined = Error.Conflict("Invoice.PaymentDeclined", "The payment provider declined the payment.");

    public static Error NotPayable(InvoiceStatus status) =>
        Error.Conflict("Invoice.NotPayable", $"The invoice is {status}; only pending invoices can be paid.");
}
