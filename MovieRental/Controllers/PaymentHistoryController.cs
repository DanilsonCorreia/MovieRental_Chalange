using Microsoft.AspNetCore.Mvc;
using MovieRental.Payment;
using MovieRental.Exceptions;

namespace MovieRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentHistoryController : ControllerBase
    {
        private readonly IPaymentFeature _paymentFeature;
        private readonly ILogger<PaymentHistoryController> _logger;

        public PaymentHistoryController(IPaymentFeature paymentFeature, ILogger<PaymentHistoryController> logger)
        {
            _paymentFeature = paymentFeature;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            try
            {
                var payments = await _paymentFeature.GetAllAsync();
                return Ok(payments);
            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogError(ex, "Failed to retrieve payments");
                return StatusCode(500, new { Message = "Failed to retrieve payments. Please try again." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            try
            {
                var payment = await _paymentFeature.GetByIdAsync(id);
                if (payment == null)
                    return NotFound(new { Message = $"Payment with ID {id} was not found." });
                
                return Ok(payment);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid payment ID: {Id}", id);
                return BadRequest(new { Message = ex.Message });
            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogError(ex, "Failed to retrieve payment with ID {Id}", id);
                return StatusCode(500, new { Message = "Failed to retrieve payment. Please try again." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetPaymentsByCustomer(int customerId)
        {
            try
            {
                var payments = await _paymentFeature.GetByCustomerIdAsync(customerId);
                return Ok(payments);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid customer ID: {CustomerId}", customerId);
                return BadRequest(new { Message = ex.Message });
            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogError(ex, "Failed to retrieve payments for customer {CustomerId}", customerId);
                return StatusCode(500, new { Message = "Failed to retrieve payments. Please try again." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("rental/{rentalId}")]
        public async Task<IActionResult> GetPaymentsByRental(int rentalId)
        {
            try
            {
                var payments = await _paymentFeature.GetByRentalIdAsync(rentalId);
                return Ok(payments);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid rental ID: {RentalId}", rentalId);
                return BadRequest(new { Message = ex.Message });
            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogError(ex, "Failed to retrieve payments for rental {RentalId}", rentalId);
                return StatusCode(500, new { Message = "Failed to retrieve payments. Please try again." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("successful")]
        public async Task<IActionResult> GetSuccessfulPayments()
        {
            try
            {
                var payments = await _paymentFeature.GetSuccessfulPaymentsAsync();
                return Ok(payments);
            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogError(ex, "Failed to retrieve successful payments");
                return StatusCode(500, new { Message = "Failed to retrieve payments. Please try again." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("failed")]
        public async Task<IActionResult> GetFailedPayments()
        {
            try
            {
                var payments = await _paymentFeature.GetFailedPaymentsAsync();
                return Ok(payments);
            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogError(ex, "Failed to retrieve failed payments");
                return StatusCode(500, new { Message = "Failed to retrieve payments. Please try again." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
} 