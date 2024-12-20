using System.ComponentModel.DataAnnotations;

namespace Models.DTO
{
    public class FridgeDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }

        [Range(-50, 50, ErrorMessage = "Temperature must be between -50 and 50 degrees.")]
        public double CurrentTemperature { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Inventory level cannot be negative.")]
        public int InventoryLevel { get; set; }
        public DateTime LastRestocked { get; set; }
    }
}
