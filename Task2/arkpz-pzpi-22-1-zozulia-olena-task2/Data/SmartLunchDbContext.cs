using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class SmartLunchDbContext : DbContext
    {
        public SmartLunchDbContext(DbContextOptions<SmartLunchDbContext> options)
            : base(options)
        {

        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<Fridge> Fridges { get; set; }
        public DbSet<FridgeInventory> FridgeInventories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>()
                .HasMany(c => c.Employees)
                .WithOne(u => u.Company)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Company>()
                .HasMany(c => c.Fridges)
                .WithOne(f => f.Company)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Fridge>()
                .HasMany(f => f.FridgeInventories)
                .WithOne(fi => fi.Fridge)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FridgeInventory>()
                .HasOne(fi => fi.FoodItem)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
