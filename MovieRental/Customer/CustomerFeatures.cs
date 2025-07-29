using Microsoft.EntityFrameworkCore;
using MovieRental.Data;

namespace MovieRental.Customer
{
	public class CustomerFeatures : ICustomerFeatures
	{
		private readonly MovieRentalDbContext _movieRentalDb;
		
		public CustomerFeatures(MovieRentalDbContext movieRentalDb)
		{
			_movieRentalDb = movieRentalDb;
		}

		public async Task<Customer> SaveAsync(Customer customer)
		{
			_movieRentalDb.Customers.Add(customer);
			await _movieRentalDb.SaveChangesAsync();
			return customer;
		}

		public async Task<Customer?> GetByIdAsync(int id)
		{
			return await _movieRentalDb.Customers.FindAsync(id);
		}

		public async Task<Customer?> GetByNameAsync(string name)
		{
			return await _movieRentalDb.Customers
				.FirstOrDefaultAsync(c => c.Name == name);
		}

		public async Task<IEnumerable<Customer>> GetAllAsync()
		{
			return await _movieRentalDb.Customers.ToListAsync();
		}
	}
} 