using AppVecinos.API.Models;

namespace AppVecinos.API.Services
{
    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetPaymentsAsync();
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<IEnumerable<Payment>> GetPaymentsByNeighborIdAsync(int neighborId);
        Task<IEnumerable<Payment>> GetPaymentsByFeeIdAsync(int feeId);
        Task<IEnumerable<Payment>> GetPaymentsByDateAsync(DateTime dateTime);      
    }
}