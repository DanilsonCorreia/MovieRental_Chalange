using Microsoft.AspNetCore.Mvc;
using MovieRental.PaymentProviders;

namespace MovieRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentProviderFactory _paymentProviderFactory;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentProviderFactory paymentProviderFactory, ILogger<PaymentController> logger)
        {
            _paymentProviderFactory = paymentProviderFactory;
            _logger = logger;
        }

        [HttpGet("methods")]
        public IActionResult GetAvailablePaymentMethods()
        {
            try
            {
                var paymentMethods = _paymentProviderFactory.GetAvailablePaymentMethods();
                return Ok(paymentMethods);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
} 