using ApexBank.Data;
using ApexBank.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ApexBank.Controllers
{
    [Authorize(Roles = "Auditor")]
    public class AuditorController(AppDbContext d) : Controller
    {
        readonly AppDbContext db = d;

        public async Task<IActionResult> Index()
        {
            ViewBag.Liquidity = await db.Accounts.SumAsync(x => x.Balance);
            ViewBag.Loans = await db.Loans.Where(x => x.Status == LoanStatus.Approved).SumAsync(x => x.PrincipalAmount);
            ViewBag.Accounts = await db.Accounts.CountAsync();
            ViewBag.Transactions = await db.Transactions.CountAsync();
            return View(await db.AuditLogs.OrderByDescending(x => x.LogDate).Take(50).ToListAsync());
        }
    }
}