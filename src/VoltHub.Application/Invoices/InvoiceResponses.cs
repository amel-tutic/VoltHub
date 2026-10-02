using VoltHub.Domain.Enums;

namespace VoltHub.Application.Invoices;

public sealed record InvoiceResponse(
    Guid Id, string InvoiceNumber, decimal Amount, DateTime IssuedAt, InvoiceStatus Status,
    Guid SessionId, string StationName, DateTime? PaidAt);

public sealed record PaymentResponse(
    Guid PaymentId, Guid InvoiceId, string InvoiceNumber, decimal Amount, PaymentMethod Method, PaymentStatus Status, DateTime? PaidAt);
