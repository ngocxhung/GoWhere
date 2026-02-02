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
    }
}