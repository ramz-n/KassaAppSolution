using Kassa.Domain.Enums;

namespace Kassa.Application.Cart
{
    public class ScannedItem
    {
        public required int ProductId { get; init; }
        public required string ProductName { get; init; }
        public required string Barcode { get; init; }
        public decimal Tax { get; init; }
        public UnitType UnitType { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal Quantity { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal GrossTotal => Math.Round(Quantity * UnitPrice, 2, MidpointRounding.AwayFromZero);

        public decimal NetTotal => Math.Max(0, GrossTotal - DiscountAmount);
    }
}
