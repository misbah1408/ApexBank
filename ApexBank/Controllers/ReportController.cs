using ApexBank.Data;
using ApexBank.Enums;
using ApexBank.Models;
using ApexBank.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApexBank.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminReportController(AppDbContext db) : Controller
    {
        private readonly AppDbContext _db = db;

        public async Task<IActionResult> Index()
        {
            var model = new AdminReportViewModel
            {

                TotalCustomers =
                    await _db.CustomerProfiles.CountAsync(),

                TotalAccounts =
                    await _db.Accounts.CountAsync(),

                TotalDeposits =
                    await _db.Transactions
                        .Where(x => x.Type == TransactionType.Deposit)
                        .SumAsync(x => (decimal?)x.Amount) ?? 0,

                TotalWithdrawals =
                    await _db.Transactions
                        .Where(x => x.Type == TransactionType.Withdrawal)
                        .SumAsync(x => (decimal?)x.Amount) ?? 0,

                TotalTransfers =
                    await _db.Transactions
                        .Where(x => x.Type == TransactionType.Transfer)
                        .SumAsync(x => (decimal?)x.Amount) ?? 0,


                TotalLoans =
                    await _db.Loans.CountAsync(),

                AppliedLoans =
                    await _db.Loans.CountAsync(
                        x => x.Status == LoanStatus.Applied),

                ApprovedLoans =
                    await _db.Loans.CountAsync(
                        x => x.Status == LoanStatus.Approved),

                RejectedLoans =
                    await _db.Loans.CountAsync(
                        x => x.Status == LoanStatus.Rejected),

                TotalApprovedLoanAmount =
                    await _db.Loans
                        .Where(x => x.Status == LoanStatus.Approved)
                        .SumAsync(x =>
                            (decimal?)x.PrincipalAmount) ?? 0,


                // --------------------------
                // REPAYMENTS
                // --------------------------

                PaidEMIs =
                    await _db.RepaymentSchedules
                        .CountAsync(x => x.Status == EmiStatus.Paid),

                UpcomingEMIs =
                    await _db.RepaymentSchedules
                        .CountAsync(x =>
                            x.Status == EmiStatus.Pending &&
                            x.DueDate >= DateTime.Today),

                OverdueEMIs =
                    await _db.RepaymentSchedules
                        .CountAsync(x =>
                            x.Status == EmiStatus.Overdue &&
                            x.DueDate < DateTime.Today),

                TotalPrincipalPaid =
                    await _db.RepaymentSchedules
                        .Where(x => x.Status == EmiStatus.Paid)
                        .SumAsync(x =>
                            (decimal?)x.PrincipalComponent) ?? 0,

                TotalInterestPaid =
                    await _db.RepaymentSchedules
                        .Where(x => x.Status == EmiStatus.Paid)
                        .SumAsync(x =>
                            (decimal?)x.InterestComponent) ?? 0
            };


            return View(model);
        }


        public async Task<IActionResult> Transactions(
            DateTime? from,
            DateTime? to,
            string? type)
        {
            if (from.HasValue && to.HasValue && from > to)
            {
                ViewBag.From = from?.ToString("yyyy-MM-dd");
                ViewBag.To = to?.ToString("yyyy-MM-dd");
                ViewBag.Type = type;
                TempData["Error"] = "To should be greater than From!!";
                return View(new List<Transaction>());
            }
            var query = _db.Transactions
                .AsQueryable();

            if (from.HasValue)
            {
                query = query.Where(x =>
                    x.TransactionDate >= from.Value.Date);
            }

            if (to.HasValue)
            {
                var endDate =
                    to.Value.Date.AddDays(1);

                query = query.Where(x =>
                    x.TransactionDate < endDate);
            }

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(x =>
                    x.Type.ToString() == type);
            }

            var transactions = await query
                .OrderByDescending(x => x.TransactionDate)
                .ToListAsync();

            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");
            ViewBag.Type = type;

            return View(transactions);
        }


        // ==============================
        // LOAN REPORT
        // ==============================

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

            var query = _db.Loans
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


        public async Task<IActionResult> Repayments(
            DateTime? from,
            DateTime? to,
            string? status)
        {
            if (from.HasValue && to.HasValue && from > to)
            {
                ViewBag.From = from?.ToString("yyyy-MM-dd");
                ViewBag.To = to?.ToString("yyyy-MM-dd");
                ViewBag.Status = status;
                TempData["Error"] = "To should be greater than From!!";
                return View(new List<RepaymentSchedule>());
            }
            var query = _db.RepaymentSchedules
                .AsQueryable();

            if (from.HasValue)
            {
                query = query.Where(x =>
                    x.DueDate >= from.Value.Date);
            }

            if (to.HasValue)
            {
                var endDate =
                    to.Value.Date.AddDays(1);

                query = query.Where(x =>
                    x.DueDate < endDate);
            }

            if (status == "Paid")
            {
                query = query.Where(x =>
                    x.Status == EmiStatus.Paid);
            }
            else if (status == "Upcoming")
            {
                query = query.Where(x =>
                    x.Status == EmiStatus.Pending &&
                    x.DueDate >= DateTime.Today);
            }
            else if (status == "Overdue")
            {
                query = query.Where(x =>
                    x.Status == EmiStatus.Overdue &&
                    x.DueDate < DateTime.Today);
            }

            var repayments = await query
                .OrderBy(x => x.DueDate)
                .ToListAsync();

            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");
            ViewBag.Status = status;

            return View(repayments);
        }

        public async Task<IActionResult> AuditLogs(
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
            var query = _db.AuditLogs
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

            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");

            return View(logs);
        }
    }
}