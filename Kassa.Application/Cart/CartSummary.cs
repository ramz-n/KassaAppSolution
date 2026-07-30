using System;
using System.Collections.Generic;
using System.Text;

namespace Kassa.Application.Cart
{
    public class CartSummary
    {
        public decimal Subtotal { get; init; }     
        public decimal TaxTotal { get; init; }
        public decimal GrandTotal { get; init; }   
        public List<TaxBreakdownItem> TaxBreakdown { get; init; } = new();
    }

    public class TaxBreakdownItem
    {
        public decimal TaxRate { get; init; }
        public decimal TaxableAmount { get; init; } 
        public decimal TaxAmount { get; init; }
    }
}
