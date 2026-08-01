using System;
using System.Collections.Generic;
using System.Text;

namespace Kassa.Domain.Entities
{
    public class TransactionLine
    {
        public int Id { get; set; }

        public int TransactionId { get; set; }
        public Transaction? Transaction { get; set; }

        public int ProductId { get; set; }

        public required string ProductName { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TaxRate { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal LineTotal { get; set; }

        public decimal TaxAmount { get; set; }
    }
}
