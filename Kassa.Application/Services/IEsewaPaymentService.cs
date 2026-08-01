namespace Kassa.Application.Services
{
    public interface IEsewaPaymentService
    {
        string BuildPaymentPayload(string merchantCode, decimal amount, string receiptNumber);
    }
}