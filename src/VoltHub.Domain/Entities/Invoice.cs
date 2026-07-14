using VoltHub.Domain.Common;
using VoltHub.Domain.Enums;

namespace VoltHub.Domain.Entities;

public sealed class Invoice : BaseEntity
{
    public string InvoiceNumber { get; private set; } = default!;
    public decimal Amount { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public Guid SessionId { get; private set; }
    public Guid UserId { get; private set; }

    private Invoice() { }

    private Invoice(Guid id, string invoiceNumber, decimal amount, Guid sessionId, Guid userId)
    {
        Id = id;
        InvoiceNumber = invoiceNumber;
        Amount = amount;
        IssuedAt = DateTime.UtcNow;
        Status = InvoiceStatus.Pending;
        SessionId = sessionId;
        UserId = userId;
    }

    // invoiceNumber ("VH-2026-000123") is passed in rather than generated here: a sequential,
    // collision-free number needs a database round-trip, which is Infrastructure's job.
    public static Invoice Create(string invoiceNumber, decimal amount, Guid sessionId, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            throw new ArgumentException("Invoice number is required.", nameof(invoiceNumber));
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");

        return new Invoice(Guid.CreateVersion7(), invoiceNumber, amount, sessionId, userId);
    }

    public void MarkPaid()
    {
        if (Status != InvoiceStatus.Pending)
            throw new InvalidOperationException($"Cannot mark an invoice with status '{Status}' as paid.");
        Status = InvoiceStatus.Paid;
    }

    public void Cancel()
    {
        if (Status != InvoiceStatus.Pending)
            throw new InvalidOperationException($"Cannot cancel an invoice with status '{Status}'.");
        Status = InvoiceStatus.Cancelled;
    }
}