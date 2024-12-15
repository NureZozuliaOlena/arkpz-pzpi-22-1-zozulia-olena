using Microsoft.EntityFrameworkCore;
using Data;
using Models;

namespace Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly SmartLunchDbContext _context;

        public NotificationRepository(SmartLunchDbContext context)
        {
            _context = context;
        }

        public async Task<Notification> GetByIdAsync(Guid id)
        {
            return await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _context.Notifications.ToListAsync();
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public async Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
        }

        public async Task DeleteAsync(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
            }
        }
    }
}
