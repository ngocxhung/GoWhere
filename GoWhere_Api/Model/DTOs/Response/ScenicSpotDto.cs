namespace GoWhere_Api.Model.DTOs.Response
{
    public class ScenicSpotDto
    {
        public int Id { get; set; }
        public string ScenicSpotName { get; set; }
        public string Description { get; set; }
        public double EstimatedVisitPrice { get; set; }
        public List<string> MediaUrls { get; set; }
        public int ProvinceId { get; set; }
    }
}