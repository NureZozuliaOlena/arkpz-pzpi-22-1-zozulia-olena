using Models.DTO;
using Models;

namespace Helpers
{
    public class MappingHelper
    {
        public static CompanyDto MapToDto(Company company)
        {
            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Address = company.Address,
                ContactEmail = company.ContactEmail,
                AdminId = company.AdminId,
                Employees = company.Employees?.Select(MapToDto).ToList()
            };
        }

        public static Company MapToEntity(CompanyDto companyDto)
        {
            return new Company
            {
                Id = companyDto.Id,
                Name = companyDto.Name,
                Address = companyDto.Address,
                ContactEmail = companyDto.ContactEmail,
                AdminId = companyDto.AdminId
            };
        }

        public static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Role = user.Role,
                CompanyId = user.CompanyId,
                Notifications = user.Notifications?.Select(MapToDto).ToList()
            };
        }

        public static User MapToEntity(UserDto userDto)
        {
            return new User
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                DateOfBirth = userDto.DateOfBirth,
                PhoneNumber = userDto.PhoneNumber,
                Email = userDto.Email,
                PasswordHash = userDto.PasswordHash,
                Role = userDto.Role,
                CompanyId = userDto.CompanyId,
                Notifications = userDto.Notifications?.Select(MapToEntity).ToList()
            };
        }

        public static FoodItemDto MapToDto(FoodItem foodItem)
        {
            return new FoodItemDto
            {
                Id = foodItem.Id,
                Name = foodItem.Name,
                Description = foodItem.Description,
                Price = foodItem.Price,
                IsAvailable = foodItem.IsAvailable
            };
        }

        public static FoodItem MapToEntity(FoodItemDto foodItemDto)
        {
            return new FoodItem
            {
                Id = foodItemDto.Id,
                Name = foodItemDto.Name,
                Description = foodItemDto.Description,
                Price = foodItemDto.Price,
                IsAvailable = foodItemDto.IsAvailable
            };
        }

        public static FridgeDto MapToDto(Fridge fridge)
        {
            return new FridgeDto
            {
                Id = fridge.Id,
                CompanyId = fridge.CompanyId,
                CurrentTemperature = fridge.CurrentTemperature,
                InventoryLevel = fridge.InventoryLevel,
                LastRestocked = fridge.LastRestocked,
                FridgeInventories = fridge.FridgeInventories?.Select(MapToDto).ToList()
            };
        }

        public static Fridge MapToEntity(FridgeDto fridgeDto)
        {
            return new Fridge
            {
                Id = fridgeDto.Id,
                CompanyId = fridgeDto.CompanyId,
                CurrentTemperature = fridgeDto.CurrentTemperature,
                InventoryLevel = fridgeDto.InventoryLevel,
                LastRestocked = fridgeDto.LastRestocked,
                FridgeInventories = fridgeDto.FridgeInventories?.Select(MapToEntity).ToList()
            };
        }

        public static FridgeInventoryDto MapToDto(FridgeInventory fridgeInventory)
        {
            return new FridgeInventoryDto
            {
                Id = fridgeInventory.Id,
                FridgeId = fridgeInventory.FridgeId,
                FoodItemId = fridgeInventory.FoodItemId,
                Quantity = fridgeInventory.Quantity
            };
        }

        public static FridgeInventory MapToEntity(FridgeInventoryDto fridgeInventoryDto)
        {
            return new FridgeInventory
            {
                Id = fridgeInventoryDto.Id,
                FridgeId = fridgeInventoryDto.FridgeId,
                FoodItemId = fridgeInventoryDto.FoodItemId,
                Quantity = fridgeInventoryDto.Quantity
            };
        }

        public static NotificationDto MapToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Text = notification.Text,
                DateTimeCreated = notification.DateTimeCreated,
                UserId = notification.UserId
            };
        }

        public static Notification MapToEntity(NotificationDto notificationDto)
        {
            return new Notification
            {
                Id = notificationDto.Id,
                Title = notificationDto.Title,
                Text = notificationDto.Text,
                DateTimeCreated = notificationDto.DateTimeCreated,
                UserId = notificationDto.UserId
            };
        }

        public static OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                FridgeId = order.FridgeId,
                TotalAmount = order.TotalAmount,
                PaymentStatus = order.PaymentStatus,
                Timestamp = order.Timestamp,
                Items = order.Items?.Select(MapToDto).ToList()
            };
        }

        public static Order MapToEntity(OrderDto orderDto)
        {
            return new Order
            {
                Id = orderDto.Id,
                UserId = orderDto.UserId,
                FridgeId = orderDto.FridgeId,
                TotalAmount = orderDto.TotalAmount,
                PaymentStatus = orderDto.PaymentStatus,
                Timestamp = orderDto.Timestamp,
                Items = orderDto.Items?.Select(MapToEntity).ToList()
            };
        }

        public static OrderItemDto MapToDto(OrderItem orderItem)
        {
            return new OrderItemDto
            {
                Id = orderItem.Id,
                OrderId = orderItem.OrderId,
                FridgeInventoryId = orderItem.FridgeInventoryId,
                Quantity = orderItem.Quantity,
                Price = orderItem.Price
            };
        }

        public static OrderItem MapToEntity(OrderItemDto orderItemDto)
        {
            return new OrderItem
            {
                Id = orderItemDto.Id,
                OrderId = orderItemDto.OrderId,
                FridgeInventoryId = orderItemDto.FridgeInventoryId,
                Quantity = orderItemDto.Quantity,
                Price = orderItemDto.Price
            };
        }
    }
}
