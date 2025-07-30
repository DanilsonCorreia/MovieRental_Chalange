using Microsoft.AspNetCore.Mvc;
using MovieRental.Customer;
using MovieRental.Exceptions;

namespace MovieRental.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CustomerController : ControllerBase
	{
		private readonly ICustomerFeatures _features;
		private readonly ILogger<CustomerController> _logger;

		public CustomerController(ICustomerFeatures features, ILogger<CustomerController> logger)
		{
			_features = features;
			_logger = logger;
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] Customer.Customer customer)
		{
			try
			{
				if (customer == null)
					return BadRequest("Customer data is required.");

				var savedCustomer = await _features.SaveAsync(customer);
				return CreatedAtAction(nameof(GetById), new { id = savedCustomer.Id }, savedCustomer);
			}
			catch (ValidationException ex)
			{
				_logger.LogWarning("Validation error: {Message}", ex.Message);
				return BadRequest(new { Message = ex.Message });
			}
			catch (DuplicateEntityException ex)
			{
				_logger.LogWarning("Duplicate customer: {Message}", ex.Message);
				return Conflict(new { Message = ex.Message });
			}
			catch (DatabaseOperationException ex)
			{
				_logger.LogError(ex, "Database operation failed");
				return StatusCode(500, new { Message = "Failed to save customer. Please try again." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				throw new Exception(ex.Message);
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var customers = await _features.GetAllAsync();
				return Ok(customers);
			}
			catch (DatabaseOperationException ex)
			{
				_logger.LogError(ex, "Failed to retrieve customers");
				return StatusCode(500, new { Message = "Failed to retrieve customers. Please try again." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				throw new Exception(ex.Message);
			}
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			try
			{
				var customer = await _features.GetByIdAsync(id);
				if (customer == null)
					return NotFound(new { Message = $"Customer with ID {id} was not found." });
				
				return Ok(customer);
			}
			catch (ArgumentException ex)
			{
				_logger.LogWarning("Invalid customer ID: {Id}", id);
				return BadRequest(new { Message = ex.Message });
			}
			catch (DatabaseOperationException ex)
			{
				_logger.LogError(ex, "Failed to retrieve customer with ID {Id}", id);
				return StatusCode(500, new { Message = "Failed to retrieve customer. Please try again." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				throw new Exception(ex.Message);
			}
		}

		[HttpGet("name/{name}")]
		public async Task<IActionResult> GetByName(string name)
		{
			try
			{
				var customer = await _features.GetByNameAsync(name);
				if (customer == null)
					return NotFound(new { Message = $"Customer '{name}' was not found." });
				
				return Ok(customer);
			}
			catch (ArgumentException ex)
			{
				_logger.LogWarning("Invalid customer name: {Name}", name);
				return BadRequest(new { Message = ex.Message });
			}
			catch (DatabaseOperationException ex)
			{
				_logger.LogError(ex, "Failed to retrieve customer with name '{Name}'", name);
				return StatusCode(500, new { Message = "Failed to retrieve customer. Please try again." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				throw new Exception(ex.Message);
			}
		}
	}
} 