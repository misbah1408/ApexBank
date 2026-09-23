
using ApexBank.Services;
using ApexBank.Data;
using Microsoft.AspNetCore.Authorization;
using ApexBank.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApexBank.ViewModels;

[Authorize(Roles = "Teller")]
public class TellerController(AppDbContext d, BankingService b, AuditService a) : Controller
{
    readonly AppDbContext db = d;
    readonly BankingService bank = b;
    readonly AuditService audit = a;
    public async Task<IActionResult> Dashboard()
    {
        ViewBag.Registrations = await db.Users.Where(x => x.Role == Role.Customer && !x.IsApproved).ToListAsync();
        ViewBag.Requests = await db.Transactions.Include(x => x.Account).Where(x => x.Status == RequestStatus.Pending).ToListAsync();
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> ApproveCustomer(int id)
    {
        var u = await db.Users.Include(x => x.CustomerProfile).FirstOrDefaultAsync(x => x.Id == id);
        if (u == null)
            return NotFound();
        u.IsApproved = true;
        u.Password = u.Name.Trim().Replace(" ", "").Substring(0, Math.Min(4, u.Name.Trim().Replace(" ", "").Length)).ToUpper() + u.DateOfBirth.Year;
        await db.SaveChangesAsync();
        await audit.LogAsync(User, "CUSTOMER", "APPROVE", $"Approved customer {u.Email}", HttpContext);
        TempData["Success"] = $"Customer approved. Credentials: {u.Name.Replace(" ", "")[..Math.Min(4, u.Name.Replace(" ", "").Length)].ToUpper()}{u.DateOfBirth.Year}";
        return RedirectToAction("Dashboard");
    }
    [HttpPost]
    public async Task<IActionResult> RequestDecision(int id, bool approve)
    {
        var ok = await bank.ApproveRequestAsync(id, approve, User.Identity!.Name!);
        TempData[ok ? "Success" : "Error"] = ok ? (approve ? "Request approved and balance updated." : "Request rejected.") : "Request not found.";
        return RedirectToAction("Dashboard");
    }
    [HttpPost]
    public async Task<IActionResult> Counter(TransactionVm m)
    {
        var t = await bank.DepositOrWithdrawAsync(m.AccountNumber, m.Amount, m.Type, User.Identity!.Name!, false);
        TempData[t == null ? "Error" : "Success"] = t == null ? "Transaction failed." : "Counter transaction completed.";
        return RedirectToAction("Dashboard");
    }
}
