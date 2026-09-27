using System.Security.Claims;

namespace ApexBank.Services.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(ClaimsPrincipal user, string module, string action, string details);
    }
}
