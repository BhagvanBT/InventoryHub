

using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add memory cache service
builder.Services.AddMemoryCache();

var app = builder.Build();

// Use CORS
app.UseCors();

// Use memory cache
var cache = app.Services.GetRequiredService<IMemoryCache>();

app.MapGet("/api/productlist", () =>
{
    const string cacheKey = "productlist";
    if (!cache.TryGetValue(cacheKey, out object? products))
    {
        products = new[]
        {
            new
            {
                Id = 1,
                Name = "Laptop",
                Price = 1200.50,
                Stock = 25,
                Category = new { Id = 101, Name = "Electronics" }
            },
            new
            {
                Id = 2,
                Name = "Headphones",
                Price = 50.00,
                Stock = 100,
                Category = new { Id = 102, Name = "Accessories" }
            }
        };
        // Cache for 5 minutes
        cache.Set(cacheKey, products, TimeSpan.FromMinutes(5));
    }
    return products;
});

Console.WriteLine("ServerApp is running...");
app.Run();