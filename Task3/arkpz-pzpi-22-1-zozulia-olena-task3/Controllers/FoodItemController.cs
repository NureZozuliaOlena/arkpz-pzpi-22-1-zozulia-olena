using Microsoft.AspNetCore.Mvc;
using Helpers;
using Models.DTO;
using Repositories;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodItemController : ControllerBase
    {
        private readonly IFoodItemRepository _foodItemRepository;

        public FoodItemController(IFoodItemRepository foodItemRepository)
        {
            _foodItemRepository = foodItemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var foodItems = await _foodItemRepository.GetAllAsync();
            var foodItemDtos = foodItems.Select(MappingHelper.MapToDto).ToList();
            return Ok(foodItemDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var foodItem = await _foodItemRepository.GetByIdAsync(id);
            if (foodItem == null)
            {
                return NotFound();
            }

            var foodItemDto = MappingHelper.MapToDto(foodItem);
            return Ok(foodItemDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FoodItemDto foodItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var foodItem = MappingHelper.MapToEntity(foodItemDto);
            await _foodItemRepository.AddAsync(foodItem);

            var createdFoodItemDto = MappingHelper.MapToDto(foodItem);
            return CreatedAtAction(nameof(GetById), new { id = createdFoodItemDto.Id }, createdFoodItemDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] FoodItemDto foodItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingFoodItem = await _foodItemRepository.GetByIdAsync(id);
            if (existingFoodItem == null)
            {
                return NotFound();
            }

            existingFoodItem.Name = foodItemDto.Name;
            existingFoodItem.Description = foodItemDto.Description;
            existingFoodItem.Price = foodItemDto.Price;
            existingFoodItem.IsAvailable = foodItemDto.IsAvailable;

            await _foodItemRepository.UpdateAsync(existingFoodItem);

            var updatedFoodItemDto = MappingHelper.MapToDto(existingFoodItem);
            return Ok(updatedFoodItemDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var foodItem = await _foodItemRepository.GetByIdAsync(id);
            if (foodItem == null)
            {
                return NotFound();
            }

            await _foodItemRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
