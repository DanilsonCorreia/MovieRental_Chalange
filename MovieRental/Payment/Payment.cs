using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieRental.Payment
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public bool IsSuccessful { get; set; }

        public string? ErrorMessage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessedAt { get; set; }

        // Foreign key relationship to Rental
        public Rental.Rental? Rental { get; set; }

        [ForeignKey("Rental")]
        public int? RentalId { get; set; }

        // Foreign key relationship to Customer
        public Customer.Customer? Customer { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        // Additional payment details
        public string? TransactionId { get; set; }

        public string? ProviderResponse { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Success, Failed, Cancelled
    }
} 