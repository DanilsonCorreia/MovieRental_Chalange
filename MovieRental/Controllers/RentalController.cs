using Microsoft.AspNetCore.Mvc;
using MovieRental.Movie;
using MovieRental.Rental;

namespace MovieRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RentalController : ControllerBase
    {
        private readonly IRentalFeatures _features;
        private readonly ILogger<RentalController> _logger;

        public RentalController(IRentalFeatures features, ILogger<RentalController> logger)
        {
            _features = features;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Rental.Rental rental)
        {
            try
            {
                return Ok(await _features.SaveAsync(rental));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
