﻿using Microsoft.EntityFrameworkCore;
using Data;
using Models;

namespace Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly SmartLunchDbContext _context;

        public OrderItemRepository(SmartLunchDbContext context)
        {
            _context = context;
        }

        public async Task<OrderItem> GetByIdAsync(Guid id)
        {
            return await _context.OrderItems
                .Include(oi => oi.FridgeInventory)
                .FirstOrDefaultAsync(oi => oi.Id == id);
        }

        public async Task<IEnumerable<OrderItem>> GetAllAsync()
        {
            return await _context.OrderItems
                .Include(oi => oi.FridgeInventory)
                .ToListAsync();
        }

        public async Task AddAsync(OrderItem orderItem)
        {
            await _context.OrderItems.AddAsync(orderItem);
        }

        public async Task UpdateAsync(OrderItem orderItem)
        {
            _context.OrderItems.Update(orderItem);
        }

        public async Task DeleteAsync(Guid id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem != null)
            {
                _context.OrderItems.Remove(orderItem);
            }
        }
    }
}
