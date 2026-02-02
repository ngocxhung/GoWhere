using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoWhere_Api.Model
{
    public class ScenicSpot
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScenicSpotId { get; set; }
        [StringLength(100)]
        public string ScenicSpotName { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }

        public double EstimatedVisitPrice { get; set; }
        
        /// <summary>
        /// Trạng thái hiển thị: true = hiển thị cho user, false = ẩn (chỉ admin thấy)
        /// </summary>
        public bool IsViewed { get; set; } = false;
        
        public ICollection<Media> Medias { get; set; }
        [ForeignKey("ProvinceId")]
        public int ProvinceId { get; set; }
        public Province Province { get; set; }
        public ICollection<CustomerSchedule> CustomerSchedules { get; set; }
    }
}