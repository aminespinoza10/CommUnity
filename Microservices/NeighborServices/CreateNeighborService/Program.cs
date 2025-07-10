using CommonData.Models;
using CommonData.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Use In-Memory database for testing/development when LocalDB is not available
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Use in-memory database for development/testing
        options.UseInMemoryDatabase("CreateNeighborServiceDb");
    }
    else
    {
        // Use SQL Server for production
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/create-neighbor", async (Neighbor neighbor, ApplicationDbContext context) =>
{
    if (neighbor == null)
    {
        return Results.BadRequest("Neighbor data is required.");
    }

    // Validate that this is not an admin user (admin users should go to CreateUserService)
    if (string.Equals(neighbor.Level, "Admin", StringComparison.OrdinalIgnoreCase))
    {
        return Results.BadRequest("Admin users should be created through the CreateUserService.");
    }

    // Validate required fields
    if (string.IsNullOrWhiteSpace(neighbor.Name) ||
        string.IsNullOrWhiteSpace(neighbor.User) ||
        string.IsNullOrWhiteSpace(neighbor.Password) ||
        string.IsNullOrWhiteSpace(neighbor.Level) ||
        string.IsNullOrWhiteSpace(neighbor.Status) ||
        neighbor.Number <= 0)
    {
        return Results.BadRequest("All neighbor fields are required and Number must be positive.");
    }

    // Check if user already exists
    var existingUser = await context.Neighbors
        .FirstOrDefaultAsync(n => n.User == neighbor.User);
    
    if (existingUser != null)
    {
        return Results.Conflict("A neighbor with this username already exists.");
    }

    // Check if apartment number is already taken
    var existingNumber = await context.Neighbors
        .FirstOrDefaultAsync(n => n.Number == neighbor.Number);
    
    if (existingNumber != null)
    {
        return Results.Conflict("A neighbor with this apartment number already exists.");
    }

    try
    {
        // Set default status if not provided
        if (string.IsNullOrWhiteSpace(neighbor.Status))
        {
            neighbor.Status = "Active";
        }

        // Add the neighbor to the database
        context.Neighbors.Add(neighbor);
        await context.SaveChangesAsync();

        // Return success response with created neighbor (excluding password for security)
        var response = new 
        {
            Message = "Neighbor created successfully",
            NeighborId = neighbor.Id,
            Name = neighbor.Name,
            Number = neighbor.Number,
            User = neighbor.User,
            Level = neighbor.Level,
            Status = neighbor.Status
        };

        return Results.Created($"/neighbor/{neighbor.Id}", response);
    }
    catch (Exception ex)
    {
        return Results.Problem($"An error occurred while creating the neighbor: {ex.Message}");
    }
})
.WithName("CreateNeighbor")
.WithOpenApi();

app.MapGet("/health", () => "CreateNeighborService is running")
.WithName("HealthCheck")
.WithOpenApi();

app.Run();
