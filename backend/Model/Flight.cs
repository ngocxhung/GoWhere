using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoWhere_Api.Model
{
    public class Flight
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FlightId { get; set; }
        public string Airline { get; set; }
        public string FromCode { get; set; }
        public string ToCode { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public double Price { get; set; }
        public string? Class { get; set; }
    }
}
