using System.Globalization;

namespace Kassa.Application.Services
{
    public class EsewaPaymentService : IEsewaPaymentService
    {
        public string BuildPaymentPayload(string merchantCode, decimal amount, string receiptNumber)
        {

            return "{"
                + $"\"merchantCode\":\"{merchantCode}\","
                + $"\"referenceId\":\"{receiptNumber}\","
                + $"\"amount\":\"{amount}\","
                + "\"currency\":\"NPR\""
                + "}";
        }
    }
}
