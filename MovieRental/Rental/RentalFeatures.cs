using Microsoft.EntityFrameworkCore;
using MovieRental.Data;

namespace MovieRental.Rental
{
	public class RentalFeatures : IRentalFeatures
	{
		private readonly MovieRentalDbContext _movieRentalDb;
		public RentalFeatures(MovieRentalDbContext movieRentalDb)
		{
			_movieRentalDb = movieRentalDb;
		}

		public async Task<Rental> SaveAsync(Rental rental)
		{
			_movieRentalDb.Rentals.Add(rental);
			await _movieRentalDb.SaveChangesAsync();
			return rental;
		}

		public async Task<IEnumerable<Rental>> GetRentalsByCustomerNameAsync(string customerName)
		{
			return await _movieRentalDb.Rentals
				.Where(r => r.Customer.Name == customerName)
				.Include(r => r.Movie)
				.Include(r => r.Customer)
				.ToListAsync();
		}
	}
}
