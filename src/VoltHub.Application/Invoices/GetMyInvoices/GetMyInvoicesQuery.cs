using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Invoices.GetMyInvoices;

public sealed record GetMyInvoicesQuery : IRequest<Result<List<InvoiceResponse>>>;
