using Microsoft.Extensions.Logging;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Domain.Enums;

namespace VoltHub.Infrastructure.Payments;

// Stands in for a real card processor / e-wallet: always approves and logs the charge.
internal sealed class SimulatedPaymentGateway(ILogger<SimulatedPaymentGateway> logger) : IPaymentGateway
{
    public Task<bool> ChargeAsync(decimal amount, PaymentMethod method, string reference, CancellationToken cancellationToken)
    {
        logger.LogInformation("Simulated {Method} payment of {Amount} for {Reference} approved", method, amount, reference);
        return Task.FromResult(true);
    }
}
