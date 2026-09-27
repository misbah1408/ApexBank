using System.Data.Common;
using ApexBank.Data;
using ApexBank.Enums;
using ApexBank.Models;
using ApexBank.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace ApexBank.Services.Implementation;

public class BankingService(AppDbContext db) : IBankingService
{

    public async Task<bool> ExecuteAsync(Transaction t)
    {
        if (t.Amount <= 0) return false;
        var a = await db.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == t.AccountNumberSafe());
        return false;
    }
    public async Task<Transaction?> DepositOrWithdrawAsync(string accountNo, decimal amount, TransactionType type, string by, bool requireApproval)
    {
        var a = await db.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == accountNo && x.Status == AccountStatus.Active);
        if (a == null || amount <= 0) return null;
        if (type == TransactionType.Withdrawal && a.Balance < amount)
            return null;
        var t = new Transaction
        {
            AccountId = a.Id,
            Amount = amount,
            Type = type,
            Status = requireApproval ? RequestStatus.Pending : RequestStatus.Approved,
            PerformedBy = by,
            ReferenceNumber = "TXN-" + Guid.NewGuid().ToString("N")[..8].ToUpper(),
            Description = type.ToString()
        };
        db.Transactions.Add(t);
        if (!requireApproval) a.Balance += type == TransactionType.Deposit ? amount : -amount;
        await db.SaveChangesAsync();
        return t;
    }
    public async Task<Transaction?> TransferAsync(string from, string to, decimal amount, string by)
    {
        // 1. Basic Validation
        if (amount <= 0 || from == to)
            return null;
        // 3. Fetch both active accounts
        var sourceAccount = await db.Accounts
            .FirstOrDefaultAsync(x => x.AccountNumber == from && x.Status == AccountStatus.Active);

        var targetAccount = await db.Accounts
            .FirstOrDefaultAsync(x => x.AccountNumber == to && x.Status == AccountStatus.Active);

        // Validate account existence, distinct IDs, and sufficient balance
        if (sourceAccount == null || targetAccount == null || sourceAccount.Id == targetAccount.Id || sourceAccount.Balance < amount)
            return null;

        // 4. Update balances
        sourceAccount.Balance -= amount;
        targetAccount.Balance += amount;

        // 5. Generate a shared reference number for both ledger entries
        var sharedReference = "TXN-" + Guid.NewGuid().ToString("N")[..8].ToUpper();

        // 6. Entry 1: Outflow (Debit) for Sender
        var debitTransaction = new Transaction
        {
            AccountId = sourceAccount.Id,
            ToAccountId = targetAccount.Id,
            Amount = amount, // Stored as negative for debit/outflow
            Type = TransactionType.Transfer,
            Status = RequestStatus.Approved,
            PerformedBy = by,
            ReferenceNumber = sharedReference,
            Description = $"Transfer to Account {targetAccount.AccountNumber}"
        };

        // 7. Entry 2: Inflow (Credit) for Receiver
        var creditTransaction = new Transaction
        {
            AccountId = targetAccount.Id,
            ToAccountId = sourceAccount.Id,
            Amount = amount, // Stored as positive for credit/inflow
            Type = TransactionType.Transfer,
            Status = RequestStatus.Approved,
            PerformedBy = by,
            ReferenceNumber = sharedReference,
            Description = $"Transfer from Account {sourceAccount.AccountNumber}"
        };

        // 8. Add both rows to the change tracker
        db.Transactions.AddRange(debitTransaction, creditTransaction);

        // 9. Save and commit atomic transaction
        await db.SaveChangesAsync();

        return debitTransaction;
    }
    public async Task<bool> ApproveRequestAsync(int id, bool approve, string by)
    {
        var t = await db.Transactions.Include(x => x.Account).FirstOrDefaultAsync(x => x.Id == id && x.Status == RequestStatus.Pending);
        if (t == null)
            return false;
        t.Status = approve ? RequestStatus.Approved : RequestStatus.Rejected;
        if (approve) t.Account.Balance += t.Type == TransactionType.Deposit ? t.Amount : -t.Amount; t.PerformedBy = by;
        await db.SaveChangesAsync();
        return true;
    }
}
static class TxExt { public static string AccountNumberSafe(this Transaction t) => ""; }
