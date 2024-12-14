namespace Models.DTO
{
    public class FridgeInventoryDto
    {
        public Guid Id { get; set; }
        public Guid FridgeId { get; set; }
        public Guid FoodItemId { get; set; }
        public int Quantity { get; set; }
    }
}
