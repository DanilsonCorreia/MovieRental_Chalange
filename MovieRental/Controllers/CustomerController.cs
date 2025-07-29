using Microsoft.AspNetCore.Mvc;
using MovieRental.Customer;

namespace MovieRental.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CustomerController : ControllerBase
	{
		private readonly ICustomerFeatures _features;

		public CustomerController(ICustomerFeatures features)
		{
			_features = features;
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] Customer.Customer customer)
		{
			return Ok(await _features.SaveAsync(customer));
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			return Ok(await _features.GetAllAsync());
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var customer = await _features.GetByIdAsync(id);
			if (customer == null)
				return NotFound();
			
			return Ok(customer);
		}

		[HttpGet("name/{name}")]
		public async Task<IActionResult> GetByName(string name)
		{
			var customer = await _features.GetByNameAsync(name);
			if (customer == null)
				return NotFound();
			
			return Ok(customer);
		}
	}
} 