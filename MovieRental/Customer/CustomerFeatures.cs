using Microsoft.EntityFrameworkCore;
using MovieRental.Data;
using MovieRental.Exceptions;

namespace MovieRental.Customer
{
	public class CustomerFeatures : ICustomerFeatures
	{
		private readonly MovieRentalDbContext _movieRentalDb;
		private readonly ILogger<CustomerFeatures> _logger;
		
		public CustomerFeatures(MovieRentalDbContext movieRentalDb, ILogger<CustomerFeatures> logger)
		{
			_movieRentalDb = movieRentalDb;
			_logger = logger;
		}

		public async Task<Customer> SaveAsync(Customer customer)
		{
			try
			{
				// Validate customer data
				if (string.IsNullOrWhiteSpace(customer.Name))
					throw new ValidationException("Customer name is required.");

				// Check for duplicate names
				var existingCustomer = await _movieRentalDb.Customers
					.FirstOrDefaultAsync(c => c.Name == customer.Name);
				if (existingCustomer != null)
					throw new DuplicateEntityException($"Customer with name '{customer.Name}' already exists.");

				_movieRentalDb.Customers.Add(customer);
				await _movieRentalDb.SaveChangesAsync();
				
				_logger.LogInformation("Customer '{CustomerName}' saved successfully with ID {CustomerId}", 
					customer.Name, customer.Id);
				
				return customer;
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError(ex, "Database error while saving customer");
				throw new DatabaseOperationException("Failed to save customer to database.", ex);
			}
			catch (Exception ex) when (ex is not ValidationException && ex is not DuplicateEntityException)
			{
				_logger.LogError(ex, "Unexpected error while saving customer");
				throw new DatabaseOperationException("An unexpected error occurred while saving the customer.", ex);
			}
		}

		public async Task<Customer?> GetByIdAsync(int id)
		{
			try
			{
				if (id <= 0)
					throw new ArgumentException("Customer ID must be greater than 0.");

				return await _movieRentalDb.Customers.FindAsync(id);
			}
			catch (Exception ex) when (ex is not ArgumentException)
			{
				_logger.LogError(ex, "Error retrieving customer with ID {CustomerId}", id);
				throw new DatabaseOperationException($"Error retrieving customer with ID {id}.", ex);
			}
		}

		public async Task<Customer?> GetByNameAsync(string name)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(name))
					throw new ArgumentException("Customer name cannot be empty.");

				return await _movieRentalDb.Customers
					.FirstOrDefaultAsync(c => c.Name == name);
			}
			catch (Exception ex) when (ex is not ArgumentException)
			{
				_logger.LogError(ex, "Error retrieving customer with name '{CustomerName}'", name);
				throw new DatabaseOperationException($"Error retrieving customer with name '{name}'.", ex);
			}
		}

		public async Task<IEnumerable<Customer>> GetAllAsync()
		{
			try
			{
				return await _movieRentalDb.Customers.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving all customers");
				throw new DatabaseOperationException("Error retrieving customers from database.", ex);
			}
		}
	}
} 