using Microsoft.EntityFrameworkCore;
using Data;
using Models;

namespace Repositories
{
    public class FridgeInventoryRepository : IFridgeInventoryRepository
    {
        private readonly SmartLunchDbContext _context;

        public FridgeInventoryRepository(SmartLunchDbContext context)
        {
            _context = context;
        }

        public async Task<FridgeInventory> GetByIdAsync(Guid id)
        {
            return await _context.FridgeInventories.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<FridgeInventory>> GetAllAsync()
        {
            return await _context.FridgeInventories.ToListAsync();
        }

        public async Task AddAsync(FridgeInventory fridgeInventory)
        {
            await _context.FridgeInventories.AddAsync(fridgeInventory);
        }

        public async Task UpdateAsync(FridgeInventory fridgeInventory)
        {
            _context.FridgeInventories.Update(fridgeInventory);
        }

        public async Task DeleteAsync(Guid id)
        {
            var fridgeInventory = await _context.FridgeInventories.FindAsync(id);
            if (fridgeInventory != null)
            {
                _context.FridgeInventories.Remove(fridgeInventory);
            }
        }
    }
}
