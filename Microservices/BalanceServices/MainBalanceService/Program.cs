using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

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

app.MapGet("/balance", (string? year, string? month) =>
{
    if(string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(month))
    {
        // Call MonthtlyBalanceService
        var response = new { ServiceTarget = "MonthtlyBalanceService" };
        return Results.Json(response);
    }
    else if(!string.IsNullOrEmpty(year) && string.IsNullOrEmpty(month))
    {
        // Call YearBalanceService
        var response = new { ServiceTarget = "YearBalanceService" };
        return Results.Json(response);
    }
    else if(string.IsNullOrEmpty(year) && string.IsNullOrEmpty(month))
    {
        //Call GeneralBalanceService
        var response = new { ServiceTarget = "GeneralBalanceService" };
        return Results.Json(response);
    }

    return Results.Json(new { ServiceTarget = "None" });
})
.WithName("MainBalanceService")
.WithOpenApi(operation => 
        {
            operation.Description = "Endpoint that returns the target service of balances.";
            return operation;
        });

app.Run();

