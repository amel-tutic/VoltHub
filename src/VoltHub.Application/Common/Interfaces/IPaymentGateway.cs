using VoltHub.Domain.Enums;

namespace VoltHub.Application.Common.Interfaces;

// The external "Platni provajder" from the SSA context diagram. Simulated in Infrastructure.
public interface IPaymentGateway
{
    Task<bool> ChargeAsync(decimal amount, PaymentMethod method, string reference, CancellationToken cancellationToken);
}
