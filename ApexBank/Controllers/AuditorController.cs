using ApexBank.Data;
using ApexBank.Enums;
using ApexBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ApexBank.Controllers
{
    [Authorize(Roles = "Auditor")]
    public class AuditorController(AppDbContext d) : Controller
    {
        readonly AppDbContext db = d;

        public async Task<IActionResult> Index(
            DateTime? from,
            DateTime? to)
        {
            if (from.HasValue && to.HasValue && from > to)
            {
                ViewBag.From = from?.ToString("yyyy-MM-dd");
                ViewBag.To = to?.ToString("yyyy-MM-dd");
                TempData["Error"] = "To should be greater than From!!";
                return View(new List<AuditLog>());
            }
            var query = db.AuditLogs
                .AsQueryable();

            if (from.HasValue)
            {
                query = query.Where(x =>
                    x.LogDate >= from.Value.Date);
            }

            if (to.HasValue)
            {
                var endDate =
                    to.Value.Date.AddDays(1);

                query = query.Where(x =>
                    x.LogDate < endDate);
            }


            var logs = await query
                .OrderByDescending(x => x.LogDate)
                .ToListAsync();
            ViewBag.Liquidity = await db.Accounts.SumAsync(x => x.Balance);
            ViewBag.Loans = await db.Loans.Where(x => x.Status == LoanStatus.Approved).SumAsync(x => x.PrincipalAmount);
            ViewBag.Accounts = await db.Accounts.CountAsync();
            ViewBag.Transactions = await db.Transactions.CountAsync();
            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");

            return View(logs);
        }
        public async Task<IActionResult> Loans(
            DateTime? from,
            DateTime? to,
            LoanStatus? status)
        {
            if (from.HasValue && to.HasValue && from > to)
            {
                ViewBag.From = from?.ToString("yyyy-MM-dd");
                ViewBag.To = to?.ToString("yyyy-MM-dd");
                ViewBag.Status = status;
                TempData["Error"] = "To should be greater than From!!";
                return View(new List<Loan>());
            }

            var query = db.Loans
                .Include(x => x.CustomerProfile)
                .ThenInclude(x => x.User)
                .AsQueryable();

            if (from.HasValue)
            {
                query = query.Where(x =>
                    x.AppliedDate >= from.Value.Date);
            }

            if (to.HasValue)
            {
                var endDate =
                    to.Value.Date.AddDays(1);

                query = query.Where(x =>
                    x.AppliedDate < endDate);
            }

            if (status.HasValue)
            {
                query = query.Where(x =>
                    x.Status == status.Value);
            }

            var loans = await query
                .OrderByDescending(x => x.AppliedDate)
                .ToListAsync();

            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");
            ViewBag.Status = status;

            return View(loans);
        }
    }
}