using Kassa.Application.Services;
using Kassa.Domain.Entities;
using Kassa.Domain.Enums;

namespace Kassa.Application.Cart
{
    public class CartService : ICartService
    {
        private readonly ITaxCalculator _taxCalculator;
        private readonly List<ScannedItem> _scannedItems = new();
        public IReadOnlyList<ScannedItem> ScannedItems => _scannedItems;
        public void RemoveScannedItem(ScannedItem scannedItem) => _scannedItems.Remove(scannedItem);
        public void Clear() => _scannedItems.Clear();

        public CartService(ITaxCalculator taxCalculator)
        {
            _taxCalculator = taxCalculator;
        }

        public ScannedItem AddProduct(Product product, decimal quantity = 1)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

            if (product.UnitType == UnitType.Piece)
            {
                var existing = _scannedItems.FirstOrDefault(l => l.ProductId == product.Id && l.DiscountAmount == 0);
                if (existing != null)
                {
                    existing.Quantity += quantity;
                    return existing;
                }
            }

            var line = new ScannedItem
            {
                ProductId = product.Id,
                ProductName = product.ProductName,
                Barcode = product.Barcode,
                UnitType = product.UnitType,
                UnitPrice = product.Price,
                Tax = product.Tax,
                Quantity = quantity
            };
            _scannedItems.Add(line);
            return line;
        }

        public void UpdateQuantity(ScannedItem scannedItem, decimal quantity)
        {
            if (quantity <= 0)
            {
                RemoveScannedItem(scannedItem);
                return;
            }
            scannedItem.Quantity = quantity;
        }

        public void ApplyScannedItemDiscount(ScannedItem scannedItem, decimal discountAmount)
        {
            if (discountAmount < 0) throw new ArgumentOutOfRangeException(nameof(discountAmount));
            scannedItem.DiscountAmount = Math.Min(discountAmount, scannedItem.GrossTotal);
        }

        public CartSummary GetSummary()
        {
            var breakdown = new Dictionary<decimal, (decimal taxable, decimal tax)>();

            foreach (var scannedItem in _scannedItems)
            {
                var taxAmount = _taxCalculator.CalculateTaxPortion(scannedItem.NetTotal, scannedItem.Tax);
                var exclusiveAmount = scannedItem.NetTotal - taxAmount;

                if (breakdown.TryGetValue(scannedItem.Tax, out var current))
                {
                    breakdown[scannedItem.Tax] = (current.taxable + exclusiveAmount, current.tax + taxAmount);
                }
                else
                {
                    breakdown[scannedItem.Tax] = (exclusiveAmount, taxAmount);
                }
            }

            var breakdownItems = breakdown
                .Select(kv => new TaxBreakdownItem { TaxRate = kv.Key, TaxableAmount = kv.Value.taxable, TaxAmount = kv.Value.tax })
                .OrderBy(b => b.TaxRate)
                .ToList();

            var subtotal = breakdownItems.Sum(b => b.TaxableAmount);
            var taxTotal = breakdownItems.Sum(b => b.TaxAmount);

            return new CartSummary
            {
                Subtotal = subtotal,
                TaxTotal = taxTotal,
                GrandTotal = subtotal + taxTotal,
                TaxBreakdown = breakdownItems
            };
        }
    }
}
