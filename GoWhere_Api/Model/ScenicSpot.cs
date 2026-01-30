using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoWhere_Api.Model
{
    public class ScenicSpot
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScenicSpotId { get; set; }
        public string ScenicSpotName { get; set; }
        public string Description { get; set; }
        public ICollection<Media> Medias { get; set; }
        [ForeignKey("ProvinceId")]
        public int ProvinceId { get; set; }
        public Province Province { get; set; }
    }
}