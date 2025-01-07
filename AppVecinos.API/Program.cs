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
builder.Services.AddScoped<IFeeService, FeeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region "Neighbors Endpoints"

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
                        operation.Description = "Endpoint that creates a new neighbor.";
                        return operation;
                    });

app.MapGet("/neighbors/{id}", async (INeighborService service, int id) =>
        {
            var result = await service.GetNeighborByIdAsync(id);
            return Results.Ok(result);
        }).WithName("GetNeighbor")
          .WithTags("Neighbors")
          .WithOpenApi(operation => 
                    {
                        operation.Description = "Endpoint that finds an specific neighbor by Id.";
                        return operation;
                    });

app.MapDelete("/neighbors/{id}", async (INeighborService service, int id) =>
        {         
            await service.DeleteNeighborAsync(id);
            return Results.Ok();
        }).WithName("DeleteNeighbor")
          .WithTags("Neighbors")
          .WithOpenApi(operation => 
                    {
                        operation.Description = "Endpoint that deletes an specific neighbor by Id.";
                        return operation;
                    });

app.MapPut("/neighbors", async (INeighborService service, Neighbor dto) =>
        {
            var result = await service.UpdateNeighborAsync(dto);
            return Results.Ok(result);
        }).WithName("UpdateNeighbor")
          .WithTags("Neighbors")
          .WithOpenApi(operation => 
                    {
                        operation.Description = "Endpoint that updates an specific neighbor.";
                        return operation;
                    });

#endregion

#region "Fees Endpoints"
app.MapGet("/fees", async (IFeeService service) => await service.GetFeesAsync())
                 .WithName("GetFees")
                 .WithTags("Fees")
                 .WithOpenApi(operation => 
                    {
                        operation.Description = "Endpoint that returns the list of feed.";
                        return operation;
                    });

app.MapPost("/fees", async (IFeeService service, Fee dto) =>
        {
            var result = await service.CreateFeeAsync(dto);
            return Results.Created($"/fees/{result.Id}", result);
        }).WithName("CreateFee")
          .WithTags("Fees")
          .WithOpenApi(operation => 
                    {
                        operation.Description = "Endpoint that creates a new fee.";
                        return operation;
                    });

app.MapGet("/fees/{id}", async (IFeeService service, int id) =>
        {
            var result = await service.GetFeeByIdAsync(id);
            return Results.Ok(result);
        }).WithName("GetFee")
          .WithTags("Fees")
          .WithOpenApi(operation => 
                    {
                        operation.Description = "Endpoint that finds an specific fee by Id.";
                        return operation;
                    });

app.MapPut("/fees", async (IFeeService service, Fee dto) =>
        {
            var result = await service.UpdateFeeAsync(dto);
            return Results.Ok(result);
        }).WithName("UpdateFee")
          .WithTags("Fees")
          .WithOpenApi(operation => 
                    {
                        operation.Description = "Endpoint that updates an specific fee.";
                        return operation;
                    });

app.MapDelete("/fees/{id}", async (IFeeService service, int id) =>
        {
            await service.DeleteFeeAsync(id);
            return Results.Ok();
        }).WithName("DeleteFee")
          .WithTags("Fees")
          .WithOpenApi(operation => 
                    {
                        operation.Description = "Endpoint that deletes an specific fee by Id.";
                        return operation;
                    });


#endregion

app.Run();
