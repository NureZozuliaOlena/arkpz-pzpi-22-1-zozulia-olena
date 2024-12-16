using System.ComponentModel.DataAnnotations;

namespace Models.DTO
{
    public class FoodItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
    }
}
