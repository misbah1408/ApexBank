using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApexBank.Enums;

namespace ApexBank.Models;

public class Transaction
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int AccountId { get; set; }

    public int? ToAccountId { get; set; }

    [Required]
    public TransactionType Type { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 9999999999999999.99)]
    public decimal Amount { get; set; }

    [Required]
    public RequestStatus Status { get; set; } = RequestStatus.Approved;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? ReferenceNumber { get; set; }

    [Required]
    public DateTime TransactionDate { get; set; } = DateTime.Now;

    [MaxLength(100)]
    public string? PerformedBy { get; set; }

    // Source account
    [ForeignKey(nameof(AccountId))]
    public Account Account { get; set; } = null!;
}