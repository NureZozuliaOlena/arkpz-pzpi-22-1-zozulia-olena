namespace SmartLunch.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid FridgeInventoryId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public FridgeInventory FridgeInventory { get; set; }
    }
}
