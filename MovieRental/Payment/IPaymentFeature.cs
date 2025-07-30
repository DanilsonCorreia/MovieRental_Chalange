using MovieRental.Data;

namespace MovieRental.Payment
{
    public interface IPaymentFeature
    {
        Task<Payment> SaveAsync(Payment payment);
        Task<Payment?> GetByIdAsync(int id);
        Task<IEnumerable<Payment>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Payment>> GetByRentalIdAsync(int rentalId);
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<IEnumerable<Payment>> GetSuccessfulPaymentsAsync();
        Task<IEnumerable<Payment>> GetFailedPaymentsAsync();
    }
} 