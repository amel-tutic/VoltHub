using VoltHub.Domain.Common;
using VoltHub.Domain.Enums;

namespace VoltHub.Domain.Entities;

public sealed class Payment : BaseEntity
{
    public PaymentMethod Method { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public Guid InvoiceId { get; private set; }

    private Payment() { }

    private Payment(Guid id, PaymentMethod method, decimal amount, Guid invoiceId)
    {
        Id = id;
        Method = method;
        Amount = amount;
        Status = PaymentStatus.Pending;
        InvoiceId = invoiceId;
    }

    public static Payment Create(PaymentMethod method, decimal amount, Guid invoiceId)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");

        return new Payment(Guid.CreateVersion7(), method, amount, invoiceId);
    }

    public void Complete()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Cannot complete a payment with status '{Status}'.");
        Status = PaymentStatus.Completed;
        PaidAt = DateTime.UtcNow;
    }

    public void Fail()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Cannot fail a payment with status '{Status}'.");
        Status = PaymentStatus.Failed;
    }
}