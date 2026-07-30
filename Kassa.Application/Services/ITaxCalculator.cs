namespace Kassa.Application.Services
{
    public interface ITaxCalculator
    {
        decimal CalculateExclusiveAmount(decimal lineTotalIncludingTax, decimal taxRate);
        decimal CalculateTaxPortion(decimal lineTotalIncludingTax, decimal taxRate);
    }
}