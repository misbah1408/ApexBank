
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApexBank.Enums;

namespace ApexBank.Models;

public class RepaymentSchedule
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int LoanId { get; set; }

    [Required]
    [Range(1, 600)]
    public int InstallmentNumber { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 9999999999999999.99)]
    public decimal EmiAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 9999999999999999.99)]
    public decimal PrincipalComponent { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 9999999999999999.99)]
    public decimal InterestComponent { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 9999999999999999.99)]
    public decimal BalanceAfterPayment { get; set; }

    [Required]
    public EmiStatus Status { get; set; } = EmiStatus.Pending;

    public DateTime? PaidDate { get; set; }

    [ForeignKey(nameof(LoanId))]
    public Loan Loan { get; set; } = null!;
}