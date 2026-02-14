using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoWhere_Api.Model
{
    public class Province
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProvinceId { get; set; }
        [StringLength(50)]
        public string ProvinceName { get; set; }
        public ICollection<ProvinceMedia> Media { get; set; }
        public ICollection<ScenicSpot> ScenicSpots { get; set; }

    }
}
