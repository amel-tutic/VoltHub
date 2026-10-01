using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoltHub.Domain.Entities;

namespace VoltHub.Infrastructure.Persistence.Configurations;

internal sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber).HasMaxLength(30);
        builder.Property(i => i.Amount).HasPrecision(10, 2);
        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);

        builder.ToTable(t => t.HasCheckConstraint("ck_invoices_amount_non_negative", "amount >= 0"));

        builder.HasIndex(i => i.InvoiceNumber).IsUnique();

        // 1:1 — one invoice per completed session (EF adds the unique index).
        builder.HasOne(i => i.Session).WithOne().HasForeignKey<Invoice>(i => i.SessionId).OnDelete(DeleteBehavior.Restrict);
    }
}
