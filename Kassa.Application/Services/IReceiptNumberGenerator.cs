namespace Kassa.Application.Services
{
    public interface IReceiptNumberGenerator
    {
        Task<string> GenerateNextAsync();
    }
}