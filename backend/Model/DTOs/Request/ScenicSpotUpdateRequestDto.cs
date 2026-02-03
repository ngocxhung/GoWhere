using System.ComponentModel.DataAnnotations;

namespace GoWhere_Api.Model.DTOs.Request
{
    public class ScenicSpotUpdateRequestDto
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Scenic spot name cannot exceed 100 characters.")]
        public string ScenicSpotName { get; set; }
        [Required]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Estimated visit price must be a non-negative value.")]
        public double EstimatedVisitPrice { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "At least one media file is required.")]
        [Url(ErrorMessage = "Invalid media file URL.")]
        
        public ICollection<string> Medias { get; set; }
        public int ProvinceId { get; set; }
    }
}
