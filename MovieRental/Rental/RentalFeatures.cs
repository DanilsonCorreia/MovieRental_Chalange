using Microsoft.EntityFrameworkCore;
using MovieRental.Data;

namespace MovieRental.Rental
{
	public class RentalFeatures : IRentalFeatures
	{
		private readonly MovieRentalDbContext _movieRentalDb;
		private readonly ILogger<RentalFeatures> _logger;
		public RentalFeatures(MovieRentalDbContext movieRentalDb, ILogger<RentalFeatures> logger)
		{
			_movieRentalDb = movieRentalDb;
			_logger = logger;
		}

		public async Task<Rental> SaveAsync(Rental rental)
		{
			try
			{
				_movieRentalDb.Rentals.Add(rental);
				await _movieRentalDb.SaveChangesAsync();
				return rental;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				throw new Exception(ex.Message);
			}
		}

		public async Task<IEnumerable<Rental>> GetRentalsByCustomerNameAsync(string customerName)
		{
			try
			{
				return await _movieRentalDb.Rentals
					.Where(r => r.Customer.Name == customerName)
					.Include(r => r.Movie)
					.Include(r => r.Customer)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				throw new Exception(ex.Message);
			}
		}
	}
}
