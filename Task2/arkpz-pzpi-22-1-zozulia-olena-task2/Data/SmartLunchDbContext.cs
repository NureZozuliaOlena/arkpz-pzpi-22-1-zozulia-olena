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
                .WithOne()
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FridgeInventory>()
                .HasOne(fi => fi.FoodItem)
                .WithMany()
                .HasForeignKey(fi => fi.FoodItemId);

            modelBuilder.Entity<FridgeInventory>()
                .HasOne<Fridge>()
                .WithMany()
                .HasForeignKey(fi => fi.FridgeId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.FridgeInventory)
                .WithMany()
                .HasForeignKey(oi => oi.FridgeInventoryId);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Order>()
                .Property(o => o.PaymentStatus)
                .HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
