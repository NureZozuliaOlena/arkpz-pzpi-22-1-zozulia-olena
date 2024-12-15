using Microsoft.EntityFrameworkCore;
using Data;
using Models;

namespace Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly SmartLunchDbContext _context;

        public CompanyRepository(SmartLunchDbContext context)
        {
            _context = context;
        }

        public async Task<Company> GetByIdAsync(Guid id)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await _context.Companies.ToListAsync();
        }

        public async Task AddAsync(Company company)
        {
            await _context.Companies.AddAsync(company);
        }

        public async Task UpdateAsync(Company company)
        {
            _context.Companies.Update(company);
        }

        public async Task DeleteAsync(Guid id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
            }
        }
    }
}
