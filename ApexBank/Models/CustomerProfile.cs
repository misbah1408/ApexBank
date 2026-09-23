using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApexBank.Models;

public class CustomerProfile
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(250)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string State { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    [RegularExpression(@"^\d{6}$")]
    public string Pincode { get; set; } = string.Empty;

    [Range(300, 900)]
    public int? CreditScore { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public ICollection<Account> Accounts { get; set; } = [];

    public ICollection<Loan> Loans { get; set; } = [];
}