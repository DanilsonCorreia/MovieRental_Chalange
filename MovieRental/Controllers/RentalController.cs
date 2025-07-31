using Microsoft.AspNetCore.Mvc;
using MovieRental.Movie;
using MovieRental.Rental;
using MovieRental.Exceptions;

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
                if (rental == null)
                    return BadRequest("Rental data is required.");

                var savedRental = await _features.SaveAsync(rental);
                return CreatedAtAction(nameof(Post), new { id = savedRental.Id }, savedRental);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validation error: {Message}", ex.Message);
                return BadRequest(new { Message = ex.Message });
            }
            catch (PaymentProcessingException ex)
            {
                _logger.LogWarning("Payment processing failed: {Message}", ex.Message);
                return BadRequest(new { Message = ex.Message });
            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogError(ex, "Database operation failed");
                return StatusCode(500, new { Message = "Failed to save rental. Please try again." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("customer/{customerName}")]
        public async Task<IActionResult> GetByCustomerName(string customerName)
        {
            try
            {
                var rentals = await _features.GetRentalsByCustomerNameAsync(customerName);
                return Ok(rentals);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid customer name: {CustomerName}", customerName);
                return BadRequest(new { Message = ex.Message });
            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogError(ex, "Failed to retrieve rentals for customer {CustomerName}", customerName);
                return StatusCode(500, new { Message = "Failed to retrieve rentals. Please try again." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
