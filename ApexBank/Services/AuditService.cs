
using System.Security.Claims;
using ApexBank.Data;
using ApexBank.Models;
namespace ApexBank.Services;

public class AuditService(AppDbContext db)
{
    private readonly AppDbContext db = db;

    public async Task LogAsync(ClaimsPrincipal user, string module, string action, string details, HttpContext? ctx = null)
    {
        db.AuditLogs.Add(new AuditLog
        {

            PerformedBy = user.Identity?.Name ?? "System",
            Role = user.FindFirstValue(ClaimTypes.Role) ?? "System",
            Module = module,
            ActionPerformed = action,
            Details = details,
            IpAddress = ctx?.Connection.RemoteIpAddress?.ToString()
        });
        await db.SaveChangesAsync();
    }
}
