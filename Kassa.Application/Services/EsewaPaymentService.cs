using System.Globalization;

namespace Kassa.Application.Services
{
    public class EsewaPaymentService : IEsewaPaymentService
    {
        public string BuildPaymentPayload(string merchantCode, decimal amount, string receiptNumber)
        {
            var amountStr = amount.ToString("F2", CultureInfo.InvariantCulture);

            return "{"
                + $"\"merchantCode\":\"{merchantCode}\","
                + $"\"referenceId\":\"{receiptNumber}\","
                + $"\"amount\":\"{amountStr}\","
                + "\"currency\":\"NPR\""
                + "}";
        }
    }
}
