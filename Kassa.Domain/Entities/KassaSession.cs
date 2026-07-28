namespace Kassa.Domain.Entities
{
    public class KassaSession
    {
        public int Id { get; set; }

        public int CashierId { get; set; }
        public Cashier? Cashier { get; set; }

        public DateTime OpenedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        public decimal StartingCash { get; set; }

        public decimal? ExpectedCash { get; set; }

        public decimal? CountedCash { get; set; }

        public decimal? CashDifference { get; set; }

        public bool IsClosed { get; set; }
    }
}
}
