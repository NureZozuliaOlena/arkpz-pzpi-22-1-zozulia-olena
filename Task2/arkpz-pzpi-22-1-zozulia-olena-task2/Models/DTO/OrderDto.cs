using Enums;

namespace Models.DTO
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid FridgeId { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime Timestamp { get; set; }
        public List<OrderItemDto>? Items { get; set; }
    }
}
