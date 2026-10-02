using FluentValidation;

namespace VoltHub.Application.Invoices.PayInvoice;

internal sealed class PayInvoiceCommandValidator : AbstractValidator<PayInvoiceCommand>
{
    public PayInvoiceCommandValidator() => RuleFor(x => x.Method).IsInEnum();
}
