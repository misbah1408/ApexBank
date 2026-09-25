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
    public class AdminController(AppDbContext d) : Controller
    {
        readonly AppDbContext db = d;

        public async Task<IActionResult> Index() => View(await db.Users.Where(x => x.Role != Role.Customer).OrderBy(x => x.Role).ToListAsync());

        public IActionResult CreateStaff() => View(new StaffVm());
        [HttpPost]
        public async Task<IActionResult> CreateStaff(StaffVm m)
        {
            if (await db.Users.AnyAsync(x => x.Email == m.Email))
            {
                ModelState.AddModelError("Email", "Email exists.");
                return View(m);
            }
            db.Users.Add(new User
            {
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                DateOfBirth = m.DateOfBirth,
                Password = m.DateOfBirth.Year.ToString(),
                Role = m.Role,
                IsApproved = true
            });
            await db.SaveChangesAsync();
            TempData["Success"] = "Staff created. Default password is birth year.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Toggle(int id)
        {
            var u = await db.Users.FindAsync(id);
            if (u != null)
            {
                u.IsActive = !u.IsActive; await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }

}
