using CornerStore.Models;
using Microsoft.EntityFrameworkCore;

public class CornerStoreDbContext : DbContext
{
    public DbSet<Cashier> Cashiers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }
    public DbSet<Product> Products { get; set; }

    public CornerStoreDbContext(DbContextOptions<CornerStoreDbContext> context)
        : base(context) { }

    //allows us to configure the schema when migrating as well as seed data
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Categories
        modelBuilder
            .Entity<Category>()
            .HasData(
                new Category[]
                {
                    new Category { Id = 1, CategoryName = "Beverages" },
                    new Category { Id = 2, CategoryName = "Snacks" },
                    new Category { Id = 3, CategoryName = "Candy" },
                }
            );

        // Products
        modelBuilder
            .Entity<Product>()
            .HasData(
                new Product[]
                {
                    new Product
                    {
                        Id = 1,
                        ProductName = "Cola",
                        Price = 1.99M,
                        Brand = "Coca-Cola",
                        CategoryId = 1,
                    },
                    new Product
                    {
                        Id = 2,
                        ProductName = "Potato Chips",
                        Price = 2.99M,
                        Brand = "Lays",
                        CategoryId = 2,
                    },
                    new Product
                    {
                        Id = 3,
                        ProductName = "Chocolate Bar",
                        Price = 1.50M,
                        Brand = "Hershey's",
                        CategoryId = 3,
                    },
                    new Product
                    {
                        Id = 4,
                        ProductName = "Energy Drink",
                        Price = 3.49M,
                        Brand = "Monster",
                        CategoryId = 1,
                    },
                    new Product
                    {
                        Id = 5,
                        ProductName = "Pretzels",
                        Price = 2.49M,
                        Brand = "Rold Gold",
                        CategoryId = 2,
                    },
                }
            );

        // Cashiers
        modelBuilder
            .Entity<Cashier>()
            .HasData(
                new Cashier[]
                {
                    new Cashier
                    {
                        Id = 1,
                        FirstName = "John",
                        LastName = "Doe",
                    },
                    new Cashier
                    {
                        Id = 2,
                        FirstName = "Jane",
                        LastName = "Smith",
                    },
                }
            );

        // Orders
        modelBuilder
            .Entity<Order>()
            .HasData(
                new Order[]
                {
                    new Order
                    {
                        Id = 1,
                        CashierId = 1,
                        PaidOnDate = DateTime.Parse("2023-07-18"),
                    },
                    new Order
                    {
                        Id = 2,
                        CashierId = 2,
                        PaidOnDate = null,
                    },
                }
            );

        // OrderProducts
        modelBuilder
            .Entity<OrderProduct>()
            .HasData(
                new OrderProduct[]
                {
                    new OrderProduct
                    {
                        Id = 1,
                        OrderId = 1,
                        ProductId = 1,
                        Quantity = 2,
                    },
                    new OrderProduct
                    {
                        Id = 2,
                        OrderId = 1,
                        ProductId = 2,
                        Quantity = 1,
                    },
                    new OrderProduct
                    {
                        Id = 3,
                        OrderId = 2,
                        ProductId = 3,
                        Quantity = 3,
                    },
                    new OrderProduct
                    {
                        Id = 4,
                        OrderId = 2,
                        ProductId = 4,
                        Quantity = 1,
                    },
                }
            );
    }
}
