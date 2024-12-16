using Microsoft.AspNetCore.Mvc;
using Helpers;
using Models.DTO;
using Repositories;

namespace SmartLunch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = users.Select(MappingHelper.MapToDto).ToList();
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = MappingHelper.MapToDto(user);
            return Ok(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = MappingHelper.MapToEntity(userDto);
            await _userRepository.AddAsync(user);

            var createdUserDto = MappingHelper.MapToDto(user);
            return CreatedAtAction(nameof(GetById), new { id = createdUserDto.Id }, createdUserDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.FirstName = userDto.FirstName;
            existingUser.LastName = userDto.LastName;
            existingUser.DateOfBirth = userDto.DateOfBirth;
            existingUser.PhoneNumber = userDto.PhoneNumber;
            existingUser.Email = userDto.Email;
            existingUser.PasswordHash = userDto.PasswordHash;
            existingUser.Role = userDto.Role;
            existingUser.CompanyId = userDto.CompanyId;

            await _userRepository.UpdateAsync(existingUser);

            var updatedUserDto = MappingHelper.MapToDto(existingUser);
            return Ok(updatedUserDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
