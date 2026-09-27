using ApexBank.Data;
using ApexBank.Models;
using ApexBank.Services.Interfaces;
using System.Security.Claims;
namespace ApexBank.Services.Implementation;

public class AuditService(AppDbContext db) : IAuditService
{
    private readonly AppDbContext db = db;

    public async Task LogAsync(ClaimsPrincipal user, string module, string action, string details)
    {
        db.AuditLogs.Add(new AuditLog
        {

            PerformedBy = user.Identity?.Name ?? "System",
            Role = user.FindFirstValue(ClaimTypes.Role) ?? "System",
            Module = module,
            ActionPerformed = action,
            Details = details
        });
        await db.SaveChangesAsync();
    }
}
