using System.ComponentModel.DataAnnotations;
using ApexBank.Enums;

namespace ApexBank.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(120)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    [Required]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    public Role Role { get; set; }

    public bool IsApproved { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public CustomerProfile? CustomerProfile { get; set; }
}