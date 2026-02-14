using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoWhere_Api.Model
{
    public class ProvinceMedia
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProvinceMediaId { get; set; }
        public string MediaUrl { get; set; }
        [ForeignKey("ProvinceId")]
        public int? ProvinceId { get; set; }
        public Province? Province { get; set; }

    }
}