using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApexBank.Enums;

namespace ApexBank.Models;

public class Account
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string AccountNumber { get; set; } = string.Empty;

    [Required]
    public int CustomerProfileId { get; set; }

    [Required]
    public AccountType AccountType { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 9999999999999999.99)]
    public decimal Balance { get; set; } = 0.00m;

    [Required]
    public AccountStatus Status { get; set; } = AccountStatus.Active;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [ForeignKey(nameof(CustomerProfileId))]
    public CustomerProfile CustomerProfile { get; set; } = null!;

    public ICollection<Transaction> Transactions { get; set; } = [];
}