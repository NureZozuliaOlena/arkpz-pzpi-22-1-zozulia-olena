using Microsoft.EntityFrameworkCore;
using Data;
using Models;

namespace Repositories
{
    public class FridgeRepository : IFridgeRepository
    {
        private readonly SmartLunchDbContext _context;

        public FridgeRepository(SmartLunchDbContext context)
        {
            _context = context;
        }

        public async Task<Fridge> GetByIdAsync(Guid id)
        {
            return await _context.Fridges.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<Fridge>> GetAllAsync()
        {
            return await _context.Fridges.ToListAsync();
        }

        public async Task AddAsync(Fridge fridge)
        {
            await _context.Fridges.AddAsync(fridge);
        }

        public async Task UpdateAsync(Fridge fridge)
        {
            _context.Fridges.Update(fridge);
        }

        public async Task DeleteAsync(Guid id)
        {
            var fridge = await _context.Fridges.FindAsync(id);
            if (fridge != null)
            {
                _context.Fridges.Remove(fridge);
            }
        }
    }
}
