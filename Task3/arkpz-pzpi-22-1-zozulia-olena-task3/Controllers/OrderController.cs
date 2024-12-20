using Microsoft.AspNetCore.Mvc;
using Helpers;
using Models.DTO;
using Repositories;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderRepository.GetAllAsync();
            var orderDtos = orders.Select(MappingHelper.MapToDto).ToList();
            return Ok(orderDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var orderDto = MappingHelper.MapToDto(order);
            return Ok(orderDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = MappingHelper.MapToEntity(orderDto);
            await _orderRepository.AddAsync(order);

            var createdOrderDto = MappingHelper.MapToDto(order);
            return CreatedAtAction(nameof(GetById), new { id = createdOrderDto.Id }, createdOrderDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingOrder = await _orderRepository.GetByIdAsync(id);
            if (existingOrder == null)
            {
                return NotFound();
            }

            existingOrder.UserId = orderDto.UserId;
            existingOrder.FridgeId = orderDto.FridgeId;
            existingOrder.TotalAmount = orderDto.TotalAmount;
            existingOrder.PaymentStatus = orderDto.PaymentStatus;
            existingOrder.Timestamp = orderDto.Timestamp;

            await _orderRepository.UpdateAsync(existingOrder);

            var updatedOrderDto = MappingHelper.MapToDto(existingOrder);
            return Ok(updatedOrderDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            await _orderRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
