using Kassa.Domain.Entities;

namespace Kassa.Application.Cart
{
    public interface ICartService
    {
        IReadOnlyList<ScannedItem> ScannedItems { get; }

        ScannedItem AddProduct(Product product, decimal quantity = 1);
        void ApplyScannedItemDiscount(ScannedItem scannedItem, decimal discountAmount);
        void Clear();
        CartSummary GetSummary();
        void RemoveScannedItem(ScannedItem scannedItem);
        void UpdateQuantity(ScannedItem scannedItem, decimal quantity);
    }
}