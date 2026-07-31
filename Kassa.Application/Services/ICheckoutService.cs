using Kassa.Application.Cart;
using Kassa.Domain.Entities;
using Kassa.Domain.Enums;

namespace Kassa.Application.Services
{
    public record CheckoutResult(bool Success, Transaction? Transaction, string? ErrorMessage);
    public interface ICheckoutService
    {
        Task<CheckoutResult> CompleteSaleAsync(ICartService cart, int cashierId, PaymentMethod paymentMethod, decimal? amountReceived);
    }
}