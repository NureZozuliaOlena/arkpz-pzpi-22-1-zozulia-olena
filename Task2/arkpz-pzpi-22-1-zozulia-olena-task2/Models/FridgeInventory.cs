namespace Models
{
    public class FridgeInventory
    {
        public Guid Id { get; set; }
        public Guid FridgeId { get; set; }
        public Guid FoodItemId { get; set; }
        public int Quantity { get; set; }
        public FoodItem FoodItem { get; set; }
    }
}
