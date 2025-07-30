using MovieRental.Data;

namespace MovieRental.Movie
{
	public class MovieFeatures : IMovieFeatures
	{
		private readonly MovieRentalDbContext _movieRentalDb;
		private readonly ILogger<MovieFeatures> _logger;
		public MovieFeatures(MovieRentalDbContext movieRentalDb, ILogger<MovieFeatures> logger)
		{
			_movieRentalDb = movieRentalDb;
			_logger = logger;
		}
		
		public Movie Save(Movie movie)
		{
			try
			{
				_movieRentalDb.Movies.Add(movie);
				_movieRentalDb.SaveChanges();
				return movie;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				throw new Exception(ex.Message);
			}
		}

		// TODO: tell us what is wrong in this method? Forget about the async, what other concerns do you have?
		public List<Movie> GetAll()
		{
			try
			{
				return _movieRentalDb.Movies.ToList();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				throw new Exception(ex.Message);
			}
		}


	}
}
