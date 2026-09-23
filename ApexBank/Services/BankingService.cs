using ApexBank.Data;
using ApexBank.Enums;
using ApexBank.Models;
using Microsoft.EntityFrameworkCore;
namespace ApexBank.Services;

public class BankingService(AppDbContext db)
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
        var a = await db.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == from && x.Status == AccountStatus.Active);
        var b = await db.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == to && x.Status == AccountStatus.Active);
        if (a == null || b == null || a.Id == b.Id || amount <= 0 || a.Balance < amount)
            return null; a.Balance -= amount; b.Balance += amount;
        var t = new Transaction
        {
            AccountId = a.Id,
            ToAccountId = b.Id,
            Amount = amount,
            Type = TransactionType.Transfer,
            Status = RequestStatus.Approved,
            PerformedBy = by,
            ReferenceNumber = "TXN-" + Guid.NewGuid().ToString("N")[..8].ToUpper(),
            Description = "Transfer"
        };
        db.Transactions.Add(t);
        await db.SaveChangesAsync();
        return t;
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
