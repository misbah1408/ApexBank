using ApexBank.Enums;
using ApexBank.Models;

namespace ApexBank.Services.Interfaces
{
    public interface IBankingService
    {
        Task<bool> ExecuteAsync(Transaction t);
        Task<Transaction?> DepositOrWithdrawAsync(string accountNo, decimal amount, TransactionType type, string by, bool requireApproval);
        Task<Transaction?> TransferAsync(string from, string to, decimal amount, string by);
        Task<bool> ApproveRequestAsync(int id, bool approve, string by);
    }
}
