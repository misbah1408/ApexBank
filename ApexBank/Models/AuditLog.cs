using System.ComponentModel.DataAnnotations;
namespace ApexBank.Models;

public class AuditLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime LogDate { get; set; } = DateTime.Now;

    [Required]
    [MaxLength(100)]
    public string PerformedBy { get; set; } = "System";

    [Required]
    [MaxLength(30)]
    public string Role { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Module { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string ActionPerformed { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Details { get; set; }

    [MaxLength(45)]
    public string? IpAddress { get; set; }
}