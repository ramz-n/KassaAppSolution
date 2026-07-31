using Kassa.Application.Cart;
using Kassa.Application.Interfaces;
using Kassa.Domain.Entities;
using Kassa.Domain.Enums;

namespace Kassa.Application.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IProductRepository _productRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITaxCalculator _taxCalculator;
        private readonly IReceiptNumberGenerator _receiptNumberGenerator;

        public CheckoutService(IProductRepository productRepository, ITransactionRepository transactionRepository, ITaxCalculator taxCalculator, IReceiptNumberGenerator receiptNumberGenerator)
        {
            _productRepository = productRepository;
            _transactionRepository = transactionRepository;
            _taxCalculator = taxCalculator;
            _receiptNumberGenerator = receiptNumberGenerator;
        }

        public async Task<CheckoutResult> CompleteSaleAsync(ICartService cart, int cashierId, PaymentMethod paymentMethod, decimal? amountReceived)
        {
            if (cart.ScannedItems.Count == 0)
                return new CheckoutResult(false, null, "Cart is empty.");

            var decremented = new List<(int productId, decimal qty)>();
            foreach (var scannedItem in cart.ScannedItems)
            {
                var product = await _productRepository.GetProductByIdAsync(scannedItem.ProductId);
                if (product is null)
                {

                    return new CheckoutResult(false, null, $"Product '{scannedItem.ProductName}' no longer exists.");
                }

                var ok = await _productRepository.TryDecrementStockAsync(product.Id, scannedItem.Quantity, []);
                if (!ok)
                {
                    return new CheckoutResult(false, null,
                        $"Not enough stock for '{scannedItem.ProductName}'. Please adjust the quantity.");
                }
                decremented.Add((product.Id, scannedItem.Quantity));
            }

            var summary = cart.GetSummary();

            var transaction = new Transaction
            {
                ReceiptNumber = await _receiptNumberGenerator.GenerateNextAsync(),
                CashierId = cashierId,
                Timestamp = DateTime.Now,
                PaymentMethod = paymentMethod,
                Subtotal = summary.Subtotal,
                TaxTotal = summary.TaxTotal,
                Total = summary.GrandTotal,
                AmountTendered = amountReceived,
                ChangeGiven = paymentMethod == PaymentMethod.Cash && amountReceived.HasValue
                    ? Math.Round(amountReceived.Value - summary.GrandTotal, 2, MidpointRounding.AwayFromZero)
                    : null
            };

            foreach (var scannedItem in cart.ScannedItems)
            {
                var taxAmount = _taxCalculator.CalculateTaxPortion(scannedItem.NetTotal, scannedItem.Tax);
            }

            await _transactionRepository.AddAsync(transaction);
            cart.Clear();

            return new CheckoutResult(true, transaction, null);
        }

    }
}
