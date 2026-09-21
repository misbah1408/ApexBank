using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApexBank.Enums;

namespace ApexBank.Models;

public class Loan
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string LoanNumber { get; set; } = string.Empty;

    [Required]
    public int CustomerProfileId { get; set; }

    [Required]
    public LoanType LoanType { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 9999999999999999.99)]
    public decimal PrincipalAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    [Range(0, 100)]
    public decimal InterestRate { get; set; }

    [Required]
    [Range(1, 600)]
    public int TenureMonths { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 9999999999999999.99)]
    public decimal EmiAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 9999999999999999.99)]
    public decimal BalanceRemaining { get; set; }

    [Required]
    public LoanStatus Status { get; set; } = LoanStatus.Applied;

    [Required]
    [Range(300, 900)]
    public int CreditScore { get; set; }

    [Required]
    [MaxLength(20)]
    public string RiskRating { get; set; } = "Medium";

    [MaxLength(1000)]
    public string? CollateralDetails { get; set; }

    [MaxLength(500)]
    public string? DocumentPath { get; set; }

    [MaxLength(500)]
    public string? RejectionReason { get; set; }

    [Required]
    public DateTime AppliedDate { get; set; } = DateTime.Now;

    public DateTime? ApprovedDate { get; set; }

    [ForeignKey(nameof(CustomerProfileId))]
    public CustomerProfile CustomerProfile { get; set; } = null!;

    public ICollection<RepaymentSchedule> RepaymentSchedules { get; set; } = [];
}