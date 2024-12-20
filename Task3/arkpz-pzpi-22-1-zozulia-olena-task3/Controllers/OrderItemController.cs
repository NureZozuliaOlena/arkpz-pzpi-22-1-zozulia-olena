using Microsoft.AspNetCore.Mvc;
using Helpers;
using Models.DTO;
using Repositories;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderItemController(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orderItems = await _orderItemRepository.GetAllAsync();
            var orderItemDtos = orderItems.Select(MappingHelper.MapToDto).ToList();
            return Ok(orderItemDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var orderItem = await _orderItemRepository.GetByIdAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            var orderItemDto = MappingHelper.MapToDto(orderItem);
            return Ok(orderItemDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderItemDto orderItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderItem = MappingHelper.MapToEntity(orderItemDto);
            await _orderItemRepository.AddAsync(orderItem);

            var createdOrderItemDto = MappingHelper.MapToDto(orderItem);
            return CreatedAtAction(nameof(GetById), new { id = createdOrderItemDto.Id }, createdOrderItemDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] OrderItemDto orderItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingOrderItem = await _orderItemRepository.GetByIdAsync(id);
            if (existingOrderItem == null)
            {
                return NotFound();
            }

            existingOrderItem.FridgeInventoryId = orderItemDto.FridgeInventoryId;
            existingOrderItem.Quantity = orderItemDto.Quantity;
            existingOrderItem.Price = orderItemDto.Price;

            await _orderItemRepository.UpdateAsync(existingOrderItem);

            var updatedOrderItemDto = MappingHelper.MapToDto(existingOrderItem);
            return Ok(updatedOrderItemDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var orderItem = await _orderItemRepository.GetByIdAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            await _orderItemRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
