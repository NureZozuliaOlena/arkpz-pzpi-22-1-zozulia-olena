using Microsoft.AspNetCore.Mvc;
using Helpers;
using Models.DTO;
using Repositories;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FridgeInventoryController : ControllerBase
    {
        private readonly IFridgeInventoryRepository _fridgeInventoryRepository;

        public FridgeInventoryController(IFridgeInventoryRepository fridgeInventoryRepository)
        {
            _fridgeInventoryRepository = fridgeInventoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var fridgeInventories = await _fridgeInventoryRepository.GetAllAsync();
            var fridgeInventoryDtos = fridgeInventories.Select(MappingHelper.MapToDto).ToList();
            return Ok(fridgeInventoryDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var fridgeInventory = await _fridgeInventoryRepository.GetByIdAsync(id);
            if (fridgeInventory == null)
            {
                return NotFound();
            }

            var fridgeInventoryDto = MappingHelper.MapToDto(fridgeInventory);
            return Ok(fridgeInventoryDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FridgeInventoryDto fridgeInventoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fridgeInventory = MappingHelper.MapToEntity(fridgeInventoryDto);
            await _fridgeInventoryRepository.AddAsync(fridgeInventory);

            var createdFridgeInventoryDto = MappingHelper.MapToDto(fridgeInventory);
            return CreatedAtAction(nameof(GetById), new { id = createdFridgeInventoryDto.Id }, createdFridgeInventoryDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] FridgeInventoryDto fridgeInventoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingFridgeInventory = await _fridgeInventoryRepository.GetByIdAsync(id);
            if (existingFridgeInventory == null)
            {
                return NotFound();
            }

            existingFridgeInventory.FridgeId = fridgeInventoryDto.FridgeId;
            existingFridgeInventory.FoodItemId = fridgeInventoryDto.FoodItemId;
            existingFridgeInventory.Quantity = fridgeInventoryDto.Quantity;

            await _fridgeInventoryRepository.UpdateAsync(existingFridgeInventory);

            var updatedFridgeInventoryDto = MappingHelper.MapToDto(existingFridgeInventory);
            return Ok(updatedFridgeInventoryDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var fridgeInventory = await _fridgeInventoryRepository.GetByIdAsync(id);
            if (fridgeInventory == null)
            {
                return NotFound();
            }

            await _fridgeInventoryRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
