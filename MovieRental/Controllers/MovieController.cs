using Microsoft.AspNetCore.Mvc;
using MovieRental.Movie;

namespace MovieRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieFeatures _features;
        private readonly ILogger<MovieController> _logger;

        public MovieController(IMovieFeatures features, ILogger<MovieController> logger)
        {
            _features = features;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_features.GetAll());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] Movie.Movie movie)
        {
            try
            {
                return Ok(_features.Save(movie));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
