using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTO;
using Repositories;

namespace Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IFridgeRepository _fridgeRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public DeviceController(IFridgeRepository fridgeRepository, INotificationRepository notificationRepository, IMapper mapper)
        {
            _fridgeRepository = fridgeRepository;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        [HttpGet("fridge/{fridgeId}")]
        public async Task<IActionResult> GetFridgeStatus(Guid fridgeId)
        {
            var fridge = await _fridgeRepository.GetByIdAsync(fridgeId);

            if (fridge == null)
            {
                return NotFound(new { message = "Fridge not found" });
            }

            var fridgeDto = _mapper.Map<FridgeDto>(fridge);

            return Ok(fridgeDto);
        }

        [HttpPost("fridge/{fridgeId}/door-status")]
        public async Task<IActionResult> UpdateFridgeDoorStatus(Guid fridgeId, [FromBody] bool isDoorOpened)
        {
            var fridge = await _fridgeRepository.GetByIdAsync(fridgeId);

            if (fridge == null)
            {
                return NotFound(new { message = "Fridge not found" });
            }

            string message = isDoorOpened ? "Fridge door opened" : "Fridge door closed";

            var notification = new Notification
            {
                Title = "Fridge Door Status",
                Text = message,
                DateTimeCreated = DateTime.UtcNow,
                FridgeId = fridge.Id
            };

            await _notificationRepository.AddAsync(notification);

            var notificationDto = _mapper.Map<NotificationDto>(notification);

            return Ok(notificationDto);
        }

        [HttpPost("fridge/{fridgeId}/status")]
        public async Task<IActionResult> UpdateFridgeStatus(Guid fridgeId, [FromBody] FridgeStatusDto fridgeStatus)
        {
            var fridge = await _fridgeRepository.GetByIdAsync(fridgeId);

            if (fridge == null)
            {
                return NotFound(new { message = "Fridge not found" });
            }

            if (fridgeStatus.CurrentTemperature < fridge.MinTemperature)
            {
                var tempAlert = new Notification
                {
                    Title = "Temperature Alert",
                    Text = $"Current temperature {fridgeStatus.CurrentTemperature} is too low, minimum is {fridge.MinTemperature}!",
                    DateTimeCreated = DateTime.UtcNow,
                    FridgeId = fridge.Id
                };
                await _notificationRepository.AddAsync(tempAlert);
            }

            if (fridgeStatus.InventoryLevel < fridge.MinInventoryLevel)
            {
                var inventoryAlert = new Notification
                {
                    Title = "Inventory Alert",
                    Text = $"Current inventory level {fridgeStatus.InventoryLevel} is too low, minimum is {fridge.MinInventoryLevel}!",
                    DateTimeCreated = DateTime.UtcNow,
                    FridgeId = fridge.Id
                };
                await _notificationRepository.AddAsync(inventoryAlert);
            }

            return Ok(new { message = "Fridge status updated and alerts checked" });
        }

    }
}
