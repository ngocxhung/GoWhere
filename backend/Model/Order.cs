using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoWhere_Api.Model
{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        [Required, Range(0, double.MaxValue)]
       
        public double TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        [Required]
        public string Status { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }

    }
    public enum OrderStatus
    {
        Pending,
        Payed,
        Cancelled
    }
}