using System.ComponentModel.DataAnnotations;
using ApexBank.Enums;
namespace ApexBank.ViewModels;

public class RegisterVm
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Phone { get; set; } = string.Empty;
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-25);
    [Required]
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
    public AccountType AccountType { get; set; } = AccountType.Savings;
}
public class LoginVm
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}
public class LoanVm
{
    public LoanType LoanType { get; set; }
    public decimal PrincipalAmount { get; set; }
    public int TenureMonths { get; set; } = 36;
    public string CollateralDetails { get; set; } = string.Empty;
    public IFormFile? Document { get; set; }
}
public class TransactionVm
{
    public string AccountNumber { get; set; } = string.Empty;
    public string? ToAccountNumber { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}
public class StaffVm
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-25);
    public Role Role { get; set; } = Role.Teller;
}
