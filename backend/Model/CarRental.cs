using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoWhere_Api.Model
{
    public class CarRental
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CarRentalId { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public int Seats { get; set; }
        public double PricePerDay { get; set; }
        public double Rating { get; set; }
        public string? ImageUrl { get; set; }
    }
}
