using System.Text.Json.Serialization;
using CornerStore.Models;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core and provides dummy value for testing
builder.Services.AddNpgsql<CornerStoreDbContext>(
    builder.Configuration["CornerStoreDbConnectionString"] ?? "testing"
);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

app.MapGet(
    "/",
    () =>
    {
        return Results.Redirect("/swagger");
    }
);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//endpoints go here
app.MapGet(
    "/cashiers/{id}",
    (CornerStoreDbContext db, int id) =>
    {
        var cashier = db
            .Cashiers.Include(o => o.Orders)
            .ThenInclude(op => op.OrderProducts)
            .ThenInclude(p => p.Product)
            .FirstOrDefault(c => c.Id == id);

        if (cashier == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(cashier);
    }
);

app.MapPost(
    "/cashiers",
    (CornerStoreDbContext db, Cashier cashier) =>
    {
        db.Cashiers.Add(cashier);
        db.SaveChanges();
        return Results.Created($"/cashiers/{cashier.Id}", cashier);
    }
);

app.MapPost(
    "/products",
    (CornerStoreDbContext db, Product product) =>
    {
        db.Products.Add(product);
        db.SaveChanges();
        return Results.Created($"/products/{product.Id}", product);
    }
);

app.MapPut(
    "/products/{id}",
    (CornerStoreDbContext db, int id, Product product) =>
    {
        Product productToUpdate = db.Products.FirstOrDefault(p => p.Id == id);

        if (productToUpdate == null)
        {
            return Results.NotFound();
        }

        productToUpdate.ProductName = product.ProductName;
        productToUpdate.Price = product.Price;
        productToUpdate.Brand = product.Brand;
        productToUpdate.CategoryId = product.CategoryId;

        db.SaveChanges();
        return Results.NoContent();
    }
);

app.MapGet(
    "/products",
    (CornerStoreDbContext db, string? search) =>
    {
        var query = db.Products.Include(cat => cat.Category).AsQueryable();

        if (search != null)
        {
            var searchLower = search.ToLower();
            query = query.Where(p =>
                p.Category.CategoryName.ToLower().Contains(searchLower)
                || p.ProductName.ToLower().Contains(searchLower)
            );
        }

        return query
            .Select(p => new Product
            {
                Id = p.Id,
                ProductName = p.ProductName,
                Price = p.Price,
                Brand = p.Brand,
                CategoryId = p.CategoryId,
                Category = new Category
                {
                    Id = p.Category.Id,
                    CategoryName = p.Category.CategoryName,
                },
            })
            .ToList();
    }
);

app.MapGet(
    "/orders/{id}",
    (CornerStoreDbContext db, int id) =>
    {
        var query = db
            .Orders.Include(c => c.Cashier)
            .Include(op => op.OrderProducts)
            .ThenInclude(p => p.Product)
            .ThenInclude(c => c.Category)
            .FirstOrDefault(order => order.Id == id);

        if (query == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(query);
    }
);

app.MapGet(
    "/orders",
    (CornerStoreDbContext db, DateTime? orderDate) =>
    {
        var query = db
            .Orders.Include(c => c.Cashier)
            .Include(op => op.OrderProducts)
            .ThenInclude(p => p.Product)
            .ThenInclude(c => c.Category)
            .AsQueryable();

        if (orderDate != null)
        {
            query = query.Where(o => o.PaidOnDate.Value.Date == orderDate.Value.Date);
        }

        return Results.Ok(query.ToList());
    }
);

app.MapDelete(
    "/orders/{id}",
    (CornerStoreDbContext db, int id) =>
    {
        var orderDelete = db.Orders.SingleOrDefault(o => o.Id == id);

        if (orderDelete == null)
        {
            return Results.NotFound();
        }

        db.Orders.Remove(orderDelete);
        db.SaveChanges();
        return Results.NoContent();
    }
);

app.Run();

//don't move or change this!
public partial class Program { }
