using Kassa.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Kassa.Domain.Entities
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string ProductName { get; set; }
        public required string  Barcode { get; set; }
        public decimal Price { get; set; }
        public UnitType UnitType { get; set; } = UnitType.Piece;
        public decimal StockQty { get; set; }
        public decimal LowStockQty { get; set; } = 3;
        public decimal Tax { get; set; }
    }
}
