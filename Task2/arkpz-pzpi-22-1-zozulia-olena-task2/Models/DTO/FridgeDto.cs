namespace Models.DTO
{
    public class FridgeDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public double CurrentTemperature { get; set; }
        public int InventoryLevel { get; set; }
        public DateTime LastRestocked { get; set; }
        public List<FridgeInventoryDto>? FridgeInventories { get; set; }
    }
}
