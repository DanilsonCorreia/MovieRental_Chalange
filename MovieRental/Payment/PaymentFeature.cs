using Microsoft.EntityFrameworkCore;
using MovieRental.Data;
using MovieRental.Exceptions;

namespace MovieRental.Payment
{
    public class PaymentFeature : IPaymentFeature
    {
        private readonly MovieRentalDbContext _movieRentalDb;
        private readonly ILogger<PaymentFeature> _logger;

        public PaymentFeature(MovieRentalDbContext movieRentalDb, ILogger<PaymentFeature> logger)
        {
            _movieRentalDb = movieRentalDb;
            _logger = logger;
        }

        public async Task<Payment> SaveAsync(Payment payment)
        {
            try
            {
                if (payment == null)
                    throw new ArgumentException("Payment cannot be null.");

                _movieRentalDb.Payments.Add(payment);
                await _movieRentalDb.SaveChangesAsync();
                
                _logger.LogInformation($"Payment saved successfully with ID {payment.Id}");
                return payment;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while saving payment");
                throw new DatabaseOperationException("Failed to save payment to database.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while saving payment");
                throw new DatabaseOperationException("An unexpected error occurred while saving the payment.", ex);
            }
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Payment ID must be greater than 0.");

                return await _movieRentalDb.Payments
                    .Include(p => p.Customer)
                    .Include(p => p.Rental)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving payment with ID {id}");
                throw new DatabaseOperationException($"Error retrieving payment with ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<Payment>> GetByCustomerIdAsync(int customerId)
        {
            try
            {
                if (customerId <= 0)
                    throw new ArgumentException("Customer ID must be greater than 0.");

                return await _movieRentalDb.Payments
                    .Where(p => p.CustomerId == customerId)
                    .Include(p => p.Customer)
                    .Include(p => p.Rental)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving payments for customer ID {customerId}");
                throw new DatabaseOperationException($"Error retrieving payments for customer ID {customerId}.", ex);
            }
        }

        public async Task<IEnumerable<Payment>> GetByRentalIdAsync(int rentalId)
        {
            try
            {
                if (rentalId <= 0)
                    throw new ArgumentException("Rental ID must be greater than 0.");

                return await _movieRentalDb.Payments
                    .Where(p => p.RentalId == rentalId)
                    .Include(p => p.Customer)
                    .Include(p => p.Rental)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving payments for rental ID {rentalId}");
                throw new DatabaseOperationException($"Error retrieving payments for rental ID {rentalId}.", ex);
            }
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            try
            {
                return await _movieRentalDb.Payments
                    .Include(p => p.Customer)
                    .Include(p => p.Rental)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all payments");
                throw new DatabaseOperationException("Error retrieving payments from database.", ex);
            }
        }

        public async Task<IEnumerable<Payment>> GetSuccessfulPaymentsAsync()
        {
            try
            {
                return await _movieRentalDb.Payments
                    .Where(p => p.IsSuccessful)
                    .Include(p => p.Customer)
                    .Include(p => p.Rental)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving successful payments");
                throw new DatabaseOperationException("Error retrieving successful payments from database.", ex);
            }
        }

        public async Task<IEnumerable<Payment>> GetFailedPaymentsAsync()
        {
            try
            {
                return await _movieRentalDb.Payments
                    .Where(p => !p.IsSuccessful)
                    .Include(p => p.Customer)
                    .Include(p => p.Rental)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving failed payments");
                throw new DatabaseOperationException("Error retrieving failed payments from database.", ex);
            }
        }
    }
} 