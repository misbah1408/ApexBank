using ApexBank.Data;
using ApexBank.Enums;
using ApexBank.Models;
using Microsoft.EntityFrameworkCore;
namespace ApexBank.Services;

public class LoanService(AppDbContext db)
{
    public static decimal CalculateEmi(decimal p, decimal annual, int months)
    {
        if (months <= 0) return 0;
        var r = (double)annual / 12 / 100;
        var n = months; if (r == 0)
            return Math.Round(p / n, 2);
        return Math.Round((decimal)((double)p * r * Math.Pow(1 + r, n) / (Math.Pow(1 + r, n) - 1)), 2);
    }
    public async Task ApproveAsync(int id, decimal rate)
    {
        var loan = await db.Loans.Include(x=>x.CustomerProfile)
                            .ThenInclude(x => x.Accounts)
                            .FirstAsync(x => x.Id == id);
        loan.Status = LoanStatus.Approved;
        loan.InterestRate = rate;
        loan.EmiAmount = CalculateEmi(loan.PrincipalAmount, rate, loan.TenureMonths);
        loan.BalanceRemaining = loan.PrincipalAmount;
        loan.ApprovedDate = DateTime.Now;
        var account = loan.CustomerProfile.Accounts.FirstOrDefault(x => x.Status == AccountStatus.Active);
        if (account != null)
            account.Balance += loan.PrincipalAmount;
        var balance = loan.PrincipalAmount;
        var monthly = rate / 12 / 100;
        for (int i = 1; i <= loan.TenureMonths; i++)
        {
            var interest = Math.Round(balance * monthly, 2);
            var principal = Math.Min(balance, loan.EmiAmount - interest);
            balance = Math.Max(0, balance - principal);
            db.RepaymentSchedules.Add(new RepaymentSchedule
            {
                LoanId = loan.Id,
                InstallmentNumber = i,
                DueDate = DateTime.Now.AddMonths(i),
                EmiAmount = loan.EmiAmount,
                InterestComponent = interest,
                PrincipalComponent = principal,
                BalanceAfterPayment = balance
            });
        }
        await db.SaveChangesAsync();
    }
    public async Task<bool> PayEmiAsync(int scheduleId)
    {
        var s = await db.RepaymentSchedules
                        .Include(x => x.Loan)
                        .ThenInclude(x => x.CustomerProfile)
                        .ThenInclude(x => x.Accounts)
                        .FirstOrDefaultAsync(x => x.Id == scheduleId && x.Status == EmiStatus.Pending);
        if (s == null)
            return false;
        var a = s.Loan.CustomerProfile.Accounts.FirstOrDefault(x => x.Status == AccountStatus.Active);
        if (a == null || a.Balance < s.EmiAmount)
            return false;
        a.Balance -= s.EmiAmount; s.Status = EmiStatus.Paid;
        s.PaidDate = DateTime.Now; s.Loan.BalanceRemaining = s.BalanceAfterPayment;
        if (s.Loan.BalanceRemaining <= 0) s.Loan.Status = LoanStatus.Closed;
        await db.SaveChangesAsync();
        return true;
    }
}
