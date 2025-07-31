using System.ComponentModel.DataAnnotations;

namespace MovieRental.Customer
{
	public class Customer
	{
		[Key]
		public int Id { get; set; }
		
		[Required]
		public string Name { get; set; } = string.Empty;
		
		public string? Email { get; set; }
		
		public string? Phone { get; set; }
		
		public string? Address { get; set; }
		
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
} 