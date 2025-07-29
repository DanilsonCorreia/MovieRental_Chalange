using System.ComponentModel.DataAnnotations;

namespace MovieRental.Customer
{
	public class Customer
	{
		[Key]
		public int Id { get; set; }
		
		[Required]
		[StringLength(100)]
		public string Name { get; set; } = string.Empty;
		
		[StringLength(200)]
		public string? Email { get; set; }
		
		[StringLength(20)]
		public string? Phone { get; set; }
		
		[StringLength(500)]
		public string? Address { get; set; }
		
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
} 