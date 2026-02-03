using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoWhere_Api.Model
{
    public class OrderItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemId { get; set; }
        public string ItemName { get; set; }
        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        [Required, Range(0, double.MaxValue)]
        public double Price { get; set; }
        [ForeignKey("OrderId")]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [ForeignKey("ScenicSpotId")]
        public int? ScenicSpotId { get; set; }
        public ScenicSpot? ScenicSpot { get; set; }

        public string? ServiceType { get; set; } // "Tour", "Hotel", "Flight", "Car", "Activity"
        public int? ServiceId { get; set; }
        public int? ParentItemId { get; set; }
    }
}