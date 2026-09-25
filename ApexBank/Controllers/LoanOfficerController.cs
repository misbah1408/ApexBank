using ApexBank.Data;
using ApexBank.Enums;
using ApexBank.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ApexBank.Controllers
{
    [Authorize(Roles = "LoanOfficer")]
    public class LoanOfficerController(AppDbContext d, LoanService s, AuditService a) : Controller
    {
        readonly AppDbContext db = d;
        readonly LoanService service = s;
        readonly AuditService audit = a;

        public async Task<IActionResult> Index() => View(await db.Loans.Include(x => x.CustomerProfile).ThenInclude(x => x.User).Where(x => x.Status == LoanStatus.Applied).OrderBy(x => x.AppliedDate).ToListAsync());
        [HttpPost]
        public async Task<IActionResult> Approve(int id, decimal interestRate)
        {
            if (interestRate <= 0) interestRate = 10;
            await service.ApproveAsync(id, interestRate);
            await audit.LogAsync(User, "LOAN", "APPROVE", $"Approved loan #{id} at {interestRate}%", HttpContext);
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
                await audit.LogAsync(User, "LOAN", "REJECT", $"Rejected loan #{id}", HttpContext);
            }
            return RedirectToAction("Index");
        }
    }

}
