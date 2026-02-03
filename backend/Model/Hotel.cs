using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoWhere_Api.Model
{
    public class Hotel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HotelId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public double PricePerNight { get; set; }
        public double Rating { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public int? ProvinceId { get; set; }
        [ForeignKey("ProvinceId")]
        public Province? Province { get; set; }
    }
}
