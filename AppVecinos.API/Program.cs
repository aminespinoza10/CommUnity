using Microsoft.EntityFrameworkCore;
using AppVecinos.API.Data;
using AppVecinos.API.Services;
using AppVecinos.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add DbContext
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Add Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<INeighborService, NeighborService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region "Neigbors Endpoints"

app.MapGet("/neighbors", async (INeighborService service) => await service.GetNeighborsAsync())
                 .WithName("GetNeighbors")
                 .WithTags("Neighbors")
                 .WithOpenApi(operation => 
                    {
                        operation.Description = "Endpoint that returns the list of neighbors.";
                        return operation;
                    });

app.MapPost("/neighbors", async (INeighborService service, Neighbor dto) =>
        {
            var result = await service.CreateNeighborAsync(dto);
            return Results.Created($"/neighbors/{result.Id}", result);
        }).WithName("CreateNeighbor")
          .WithTags("Neighbors")
          .WithOpenApi(operation => 
                    {
                        operation.Description = "Endpoint that create a new neighbor.";
                        return operation;
                    });

app.MapGet("/neighbors/{id}", async (INeighborService service, int id) =>
        {
            var result = await service.GetNeighborByIdAsync(id);
            return result != null ? Results.Ok(result) : Results.NotFound($"Neighbor with ID {id} not found.");
        }).WithName("GetNeighbor")
          .WithTags("Neighbors")
          .WithOpenApi(operation => 
                    {
                        operation.Description = "Endpoint that find an specific neighbor by Id.";
                        return operation;
                    });

app.MapDelete("/neighbors/{id}", async (INeighborService service, int id) =>
        {
            await service.DeleteNeighborAsync(id);
            return Results.NoContent();
        }).WithName("DeleteNeighbor")
          .WithTags("Neighbors")
          .WithOpenApi(operation => 
                    {
                        operation.Description = "Endpoint that delete an specific neighbor by Id.";
                        return operation;
                    });

#endregion

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
