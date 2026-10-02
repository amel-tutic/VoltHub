using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltHub.Api.Contracts;
using VoltHub.Application.Common.Security;
using VoltHub.Application.Invoices.GetMyInvoices;
using VoltHub.Application.Invoices.PayInvoice;

namespace VoltHub.Api.Controllers;

[Route("api/invoices")]
[Authorize(Roles = Roles.User)]
public sealed class InvoicesController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetMyInvoicesQuery(), cancellationToken));

    [HttpPost("{id:guid}/pay")]
    public async Task<IActionResult> Pay(Guid id, PayInvoiceRequest request, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new PayInvoiceCommand(id, request.Method), cancellationToken));
}
