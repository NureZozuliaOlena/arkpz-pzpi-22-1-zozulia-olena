namespace SmartLunch.Models
{
    public class Fridge
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public double CurrentTemperature { get; set; }
        public int InventoryLevel { get; set; }
        public DateTime LastRestocked { get; set; }
    }
}
