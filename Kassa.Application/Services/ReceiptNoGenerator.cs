using Kassa.Application.Interfaces;

namespace Kassa.Application.Services
{
    public class ReceiptNumberGenerator : IReceiptNumberGenerator
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly string _storeCode;

        public ReceiptNumberGenerator(ITransactionRepository transactionRepository, string storeCode = "STORE001")
        {
            _transactionRepository = transactionRepository;
            _storeCode = storeCode;
        }

        public async Task<string> GenerateNextAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var countSoFar = await _transactionRepository.CountForDateAsync(today);
            var nextSequence = countSoFar + 1;
            return $"{_storeCode}-{today:yyyyMMdd}-{nextSequence:D4}";
        }
    }
}
