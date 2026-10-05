using ApexBank.Data;
using ApexBank.Enums;
using ApexBank.Models;
using ApexBank.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace ApexBank.Controllers;

public class AccountController(AppDbContext db, IHttpClientFactory factory) : Controller
{
    readonly AppDbContext db = db;
    private readonly HttpClient _httpClient = factory.CreateClient("BankApi");

    [AllowAnonymous]
    public IActionResult Login() => View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginVm m)
    {
        if (!ModelState.IsValid)
            return View(m);

        var response = await _httpClient.PostAsJsonAsync("api/auth/getuser", m);
        // 1. Check if the API rejected the login
        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Invalid credentials.");
            return View(m);
        }
        // 2. Read the returned user data
        var u = await response.Content.ReadFromJsonAsync<User>();

        if (u == null)
        {
            ModelState.AddModelError("", "Invalid credentials.");
            return View(m);
        }
        // 3. Create security claims (the user's identity card)
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, u.Id.ToString()),
        new Claim(ClaimTypes.Name, u.Name),
        new Claim(ClaimTypes.Email, u.Email),
        new Claim(ClaimTypes.Role, u.Role.ToString())
    };
        // 4. Issue the local authentication cookie
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme)));

        return u.Role switch
        {
            Role.Customer => RedirectToAction("Dashboard", "Customer"),
            Role.Teller => RedirectToAction("Dashboard", "Teller"),
            Role.LoanOfficer => RedirectToAction("Index", "LoanOfficer"),
            Role.Auditor => RedirectToAction("Index", "Auditor"),
            Role.Admin => RedirectToAction("Index", "Admin"),
            _ => RedirectToAction("Login")
        };
    }

    [AllowAnonymous]
    public IActionResult Register() => View(new RegisterVm());

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Register(RegisterVm m)
    {
        if (!ModelState.IsValid)
            return View(m);

        if (await db.Users.AnyAsync(x => x.Email == m.Email))
        {
            ModelState.AddModelError("Email", "Email already registered.");
            return View(m);
        }
        var u = new User
        {
            Name = m.Name,
            Email = m.Email,
            Phone = m.Phone,
            DateOfBirth = m.DateOfBirth,
            Password = "PENDING",
            Role = Role.Customer,
            IsApproved = false
        };
        u.CustomerProfile = new CustomerProfile
        {
            Address = m.Address,
            City = m.City,
            State = m.State,
            Pincode = m.Pincode
        }; db.Users.Add(u);

        await db.SaveChangesAsync();
        db.Accounts.Add(new Account
        {
            CustomerProfileId = u.CustomerProfile!.Id,
            AccountNumber = "ACC-" + Random.Shared.Next(10000000, 99999999),
            AccountType = m.AccountType,
            Balance = 0,
            Status = AccountStatus.Active
        });
        await db.SaveChangesAsync();
        TempData["Success"] = "Registration submitted. Wait for Teller approval to receive login credentials.";
        return RedirectToAction("Login");
    }
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Login");
    }
    [AllowAnonymous]
    public IActionResult AccessDenied() => View();
}
