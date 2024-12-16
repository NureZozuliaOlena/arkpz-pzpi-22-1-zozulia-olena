using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Fridge
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
        public double CurrentTemperature { get; set; }
        public int InventoryLevel { get; set; }
        public DateTime LastRestocked { get; set; }
        public ICollection<FridgeInventory>? FridgeInventories { get; set; } = new List<FridgeInventory>();
    }
}
