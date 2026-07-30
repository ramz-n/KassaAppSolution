namespace Kassa.Application.Services
{
    public class TaxCalculator : ITaxCalculator
    {
        private const int DecimalPlaces = 2;

        public decimal CalculateTaxPortion(decimal lineTotalIncludingTax, decimal taxRate)
        {
            if (taxRate < 0) throw new ArgumentOutOfRangeException(nameof(taxRate));
            if (taxRate == 0) return 0m;

            var exclusive = CalculateExclusiveAmount(lineTotalIncludingTax, taxRate);
            var tax = lineTotalIncludingTax - exclusive;
            return Math.Round(tax, DecimalPlaces, MidpointRounding.AwayFromZero);
        }

        public decimal CalculateExclusiveAmount(decimal lineTotalIncludingTax, decimal taxRate)
        {
            if (taxRate < 0) throw new ArgumentOutOfRangeException(nameof(taxRate));
            var exclusive = lineTotalIncludingTax / (1 + taxRate);
            return Math.Round(exclusive, DecimalPlaces, MidpointRounding.AwayFromZero);
        }
    }
}
