using Kassa.Domain.Enums;

namespace Kassa.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public required string ReceiptNumber { get; set; }
        public int CashierId { get; set; }
        public Cashier? Cashier { get; set; }
        public DateTime Timestamp { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Subtotal { get; set; } 
        public decimal TaxTotal { get; set; }   
        public decimal Total { get; set; }  
        public decimal? AmountTendered { get; set; }
        public decimal? ChangeGiven { get; set; }
        public List<TransactionLine> Lines { get; set; } = new();
    }
}
