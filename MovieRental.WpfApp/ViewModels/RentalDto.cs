using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRental.WpfApp.ViewModels
{
    public class RentalDto
    {
        public int Id { get; set; }
        public int DaysRented { get; set; }

        public int MovieId { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public int CustomerId { get; set; }
    }
}
