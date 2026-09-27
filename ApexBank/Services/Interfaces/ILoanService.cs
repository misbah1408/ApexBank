namespace ApexBank.Services.Interfaces
{
    public interface ILoanService
    {
        Task ApproveAsync(int id, decimal rate);
        Task<bool> PayEmiAsync(int scheduleId);
    }
}
