using System.ComponentModel.DataAnnotations;

namespace Models.DTO
{
    public class FridgeInventoryDto
    {
        public Guid Id { get; set; }
        public Guid FridgeId { get; set; }
        public Guid? FoodItemId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int Quantity { get; set; }
    }
}
