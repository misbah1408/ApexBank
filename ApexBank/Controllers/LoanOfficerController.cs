using ApexBank.Data;
using ApexBank.Enums;
using ApexBank.Services.Implementation;
using ApexBank.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ApexBank.Controllers
{
    [Authorize(Roles = "LoanOfficer")]
    public class LoanOfficerController(AppDbContext d, ILoanService s, IAuditService a) : Controller
    {
        readonly AppDbContext db = d;
        readonly ILoanService service = s;
        readonly IAuditService audit = a;

        public async Task<IActionResult> Index() => View(await db.Loans.Include(x => x.CustomerProfile).ThenInclude(x => x.User).Where(x => x.Status == LoanStatus.Applied).OrderBy(x => x.AppliedDate).ToListAsync());
        [HttpPost]
        public async Task<IActionResult> Approve(int id, decimal interestRate)
        {
            if (interestRate <= 0) interestRate = 10;
            await service.ApproveAsync(id, interestRate);
            await audit.LogAsync(User, "LOAN", "APPROVE", $"Approved loan #{id} at {interestRate}%");
            TempData["Success"] = "Loan approved, disbursed and EMI schedule generated.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var l = await db.Loans.FindAsync(id);
            if (l != null)
            {
                l.Status = LoanStatus.Rejected;
                l.RejectionReason = reason;
                await db.SaveChangesAsync();
                await audit.LogAsync(User, "LOAN", "REJECT", $"Rejected loan #{id}");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DownloadPdf(int id)
        {
            var loan = await db.Loans.FindAsync(id);

            if (loan == null || loan.FileData == null || loan.FileData.Length == 0)
            {
                return NotFound("PDF not found.");
            }

            return File(
                loan.FileData,
                "application/pdf",
                $"LoanDocument_{loan.Id}.pdf"
            );
        }
    }
}
