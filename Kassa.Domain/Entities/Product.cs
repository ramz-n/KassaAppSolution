using System;
using System.Collections.Generic;
using System.Text;

namespace Kassa.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public required string ProductName { get; set; }
        public required string  Barcode { get; set; }
        public decimal Price { get; set; }
        public decimal StockQty { get; set; }
        public decimal LowStockQty { get; set; } = 3;
        public decimal Tax { get; set; }
    }
}
