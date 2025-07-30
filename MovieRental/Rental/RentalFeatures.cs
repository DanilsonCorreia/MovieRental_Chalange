using Microsoft.EntityFrameworkCore;
using MovieRental.Data;
using MovieRental.PaymentProviders;
using MovieRental.Exceptions;
using MovieRental.Payment;

namespace MovieRental.Rental
{
	public class RentalFeatures : IRentalFeatures
	{
		private readonly MovieRentalDbContext _movieRentalDb;
		private readonly IPaymentProviderFactory _paymentProviderFactory;
		private readonly IPaymentFeature _paymentFeature;
		private readonly ILogger<RentalFeatures> _logger;
		
		public RentalFeatures(MovieRentalDbContext movieRentalDb, IPaymentProviderFactory paymentProviderFactory, IPaymentFeature paymentFeature, ILogger<RentalFeatures> logger)
		{
			_movieRentalDb = movieRentalDb;
			_paymentProviderFactory = paymentProviderFactory;
			_paymentFeature = paymentFeature;
			_logger = logger;
		}

		public async Task<Rental> SaveAsync(Rental rental)
		{
			try
			{
				// Validate rental data
				if (rental == null)
					throw new ArgumentException("Rental cannot be null.");

				if (rental.CustomerId <= 0)
					throw new ValidationException("Customer ID is required.");

				if (rental.MovieId <= 0)
					throw new ValidationException("Movie ID is required.");

				if (rental.DaysRented <= 0)
					throw new ValidationException("Days rented must be greater than 0.");

				// Process payment before saving rental
				_logger.LogInformation($"Processing payment for rental with customer {rental.CustomerId} and movie {rental.MovieId}");

				var price = CalculateRentalPrice(rental);
				var paymentSuccessful = await ProcessPaymentAsync(rental, price);

				if (!paymentSuccessful)
				{
					_logger.LogWarning("Payment failed for rental. Rental will not be saved.");
					throw new PaymentProcessingException("Payment processing failed. Rental cannot be created.");
				}

				// Payment successful, save the rental
				_movieRentalDb.Rentals.Add(rental);
				await _movieRentalDb.SaveChangesAsync();
				
				// Link the successful payment to the rental
				var payment = await _movieRentalDb.Payments
					.Where(p => p.CustomerId == rental.CustomerId && 
							   p.PaymentMethod == rental.PaymentMethod && 
							   p.IsSuccessful == true)
					.OrderByDescending(p => p.CreatedAt)
					.FirstOrDefaultAsync();
				
				if (payment != null)
				{
					payment.RentalId = rental.Id;
					await _movieRentalDb.SaveChangesAsync();
				}
				
				_logger.LogInformation($"Rental saved successfully with ID {rental.Id}" );
				return rental;
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError(ex, "Database error while saving rental");
				throw new DatabaseOperationException("Failed to save rental to database.", ex);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error while saving rental");
				throw new DatabaseOperationException("An unexpected error occurred while saving the rental.", ex);
			}
		}

		public async Task<IEnumerable<Rental>> GetRentalsByCustomerNameAsync(string customerName)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(customerName))
					throw new ArgumentException("Customer name cannot be empty.");

				return await _movieRentalDb.Rentals
					.Where(r => r.Customer.Name == customerName)
					.Include(r => r.Movie)
					.Include(r => r.Customer)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error retrieving rentals for customer {customerName}");
				throw new DatabaseOperationException($"Error retrieving rentals for customer '{customerName}'.", ex);
			}
		}

		private async Task<bool> ProcessPaymentAsync(Rental rental, double price)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(rental.PaymentMethod))
					throw new ValidationException("Payment method is required.");

				var paymentProvider = _paymentProviderFactory.GetPaymentProvider(rental.PaymentMethod);
				var paymentResult = await paymentProvider.ProcessPaymentAsync(price);

				if (paymentResult)
				{
					// Only save payment record if payment is successful
					var payment = new Payment.Payment
					{
						PaymentMethod = rental.PaymentMethod,
						Amount = (decimal)price,
						CustomerId = rental.CustomerId,
						IsSuccessful = true,
						Status = "Success",
						CreatedAt = DateTime.UtcNow,
						ProcessedAt = DateTime.UtcNow,
						TransactionId = Guid.NewGuid().ToString(), 
						ProviderResponse = "Payment processed successfully"
					};

					await _paymentFeature.SaveAsync(payment);
					_logger.LogInformation($"Payment successful for rental {rental.Id}");
				}
				else
				{
					_logger.LogWarning($"Payment failed for rental {rental.Id} using {rental.PaymentMethod}");
				}

				return paymentResult;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error processing payment for rental {rental.Id}");
				throw new PaymentProcessingException($"Payment processing failed: {ex.Message}");
			}
		}

		private double CalculateRentalPrice(Rental rental)
		{
			// Simple pricing model: $5 per day
			const double pricePerDay = 5.0;
			return rental.DaysRented * pricePerDay;
		}
	}
}
