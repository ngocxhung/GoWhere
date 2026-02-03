namespace GoWhere_Api.Model.DTOs.Request
{
    public class AddToCartDto
    {
        public string ItemName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; } = 1;
        public int? ScenicSpotId { get; set; }
        public string? ServiceType { get; set; }
        public int? ServiceId { get; set; }
        public int? ParentItemId { get; set; }
    }
}
