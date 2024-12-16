using Microsoft.AspNetCore.Mvc;
using Helpers;
using Models.DTO;
using Repositories;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notifications = await _notificationRepository.GetAllAsync();
            var notificationDtos = notifications.Select(MappingHelper.MapToDto).ToList();
            return Ok(notificationDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            var notificationDto = MappingHelper.MapToDto(notification);
            return Ok(notificationDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationDto notificationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var notification = MappingHelper.MapToEntity(notificationDto);
            await _notificationRepository.AddAsync(notification);

            var createdNotificationDto = MappingHelper.MapToDto(notification);
            return CreatedAtAction(nameof(GetById), new { id = createdNotificationDto.Id }, createdNotificationDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] NotificationDto notificationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingNotification = await _notificationRepository.GetByIdAsync(id);
            if (existingNotification == null)
            {
                return NotFound();
            }

            existingNotification.Title = notificationDto.Title;
            existingNotification.Text = notificationDto.Text;
            existingNotification.DateTimeCreated = notificationDto.DateTimeCreated;
            existingNotification.UserId = notificationDto.UserId;

            await _notificationRepository.UpdateAsync(existingNotification);

            var updatedNotificationDto = MappingHelper.MapToDto(existingNotification);
            return Ok(updatedNotificationDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            await _notificationRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
