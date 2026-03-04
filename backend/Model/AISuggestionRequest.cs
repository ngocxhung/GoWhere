using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoWhere_Api.Model
{
    public class AISuggestionRequest
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public int AllowedCount { get; set; } = 2; // Default to 2 free turns
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User User { get; set; }
        [ForeignKey("OrderId")]
        public int? OrderId { get; set; }
        public Order? Order { get; set; }


    }
}
