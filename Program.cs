using Microsoft.EntityFrameworkCore;
using RailwayPostgresAPI.Data;
using RailwayPostgresAPI.Repositories.Implementation;
using RailwayPostgresAPI.Repositories.Interfaces;
using RailwayPostgresAPI.Services.Implementation;
using RailwayPostgresAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database connection
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
Console.WriteLine($"DATABASE_URL present: {databaseUrl != null}");

if (!string.IsNullOrEmpty(databaseUrl))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};" +
        $"Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";

    Console.WriteLine($"Connecting to database at {uri.Host}:{uri.Port}");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    Console.WriteLine("Using local connection string");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Apply migrations and ensure database is ready
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        Console.WriteLine("Starting database migration...");
        dbContext.Database.Migrate();
        Console.WriteLine("Database migration completed successfully");
        Console.WriteLine("Products table should now exist");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during migration: {ex.Message}");
        Console.WriteLine("Attempting to create database/tables as fallback...");
        dbContext.Database.EnsureCreated();
        Console.WriteLine("EnsureCreated completed");
    }
}

app.Run();