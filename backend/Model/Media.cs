using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoWhere_Api.Model
{
    public class Media
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MediaId { get; set; }
        public string MediaUrl { get; set; }
        [ForeignKey("ScenicSpotId")]
        public int ScenicSpotId { get; set; }

        public ScenicSpot ScenicSpot { get; set; }
    }
}