using Microsoft.AspNetCore.Mvc;
using Helpers;
using Models.DTO;
using Repositories;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FridgeController : ControllerBase
    {
        private readonly IFridgeRepository _fridgeRepository;

        public FridgeController(IFridgeRepository fridgeRepository)
        {
            _fridgeRepository = fridgeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var fridges = await _fridgeRepository.GetAllAsync();
            var fridgeDtos = fridges.Select(MappingHelper.MapToDto).ToList();
            return Ok(fridgeDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var fridge = await _fridgeRepository.GetByIdAsync(id);
            if (fridge == null)
            {
                return NotFound();
            }

            var fridgeDto = MappingHelper.MapToDto(fridge);
            return Ok(fridgeDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FridgeDto fridgeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fridge = MappingHelper.MapToEntity(fridgeDto);
            await _fridgeRepository.AddAsync(fridge);

            var createdFridgeDto = MappingHelper.MapToDto(fridge);
            return CreatedAtAction(nameof(GetById), new { id = createdFridgeDto.Id }, createdFridgeDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] FridgeDto fridgeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingFridge = await _fridgeRepository.GetByIdAsync(id);
            if (existingFridge == null)
            {
                return NotFound();
            }

            existingFridge.CompanyId = fridgeDto.CompanyId;
            existingFridge.CurrentTemperature = fridgeDto.CurrentTemperature;
            existingFridge.InventoryLevel = fridgeDto.InventoryLevel;
            existingFridge.LastRestocked = fridgeDto.LastRestocked;

            await _fridgeRepository.UpdateAsync(existingFridge);

            var updatedFridgeDto = MappingHelper.MapToDto(existingFridge);
            return Ok(updatedFridgeDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var fridge = await _fridgeRepository.GetByIdAsync(id);
            if (fridge == null)
            {
                return NotFound();
            }

            await _fridgeRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
