using Microsoft.EntityFrameworkCore;
using AppVecinos.API.Data;
using AppVecinos.API.Services;
using AppVecinos.API.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;



var builder = WebApplication.CreateBuilder(args);

// Token JWT configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];


// JWT authentication parameters
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
});


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
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IOutcomeService, OutcomeService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login", async (INeighborService service, LoginRequest loginRequest) =>
{
     var result = await service.GetNeighborByCredentialsAsync(loginRequest.Username, loginRequest.Password);
    if (result != null){
        var token = GenerateJwtToken(loginRequest.Username, result.Level);
        Console.WriteLine($"Token generado: {token}");
        return Results.Ok(new { Token = token }); 
    }
    else
    {
        return Results.Unauthorized();
    }
});


#region "Neighbors Endpoints"

app.MapGet("/neighbors", async (INeighborService service) => await service.GetNeighborsAsync())
            .WithName("GetNeighbors")
            .WithTags("Neighbors")
            .WithOpenApi(operation => 
            {
                operation.Description = "Endpoint that returns the list of neighbors.";
                return operation;
            })
            .RequireAuthorization("AdminPolicy");

app.MapPost("/neighbors", async (INeighborService service, Neighbor model) =>
        {
            var result = await service.CreateNeighborAsync(model);
            return Results.Created($"/neighbors/{result.Id}", result);
        }).WithName("CreateNeighbor")
          .WithTags("Neighbors")
          .WithOpenApi(operation => 
            {
                operation.Description = "Endpoint that creates a new neighbor.";
                return operation;
            })
            .RequireAuthorization("AdminPolicy");

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
            })
            .RequireAuthorization("AdminPolicy");

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
            })
            .RequireAuthorization("AdminPolicy");

app.MapPut("/neighbors", async (INeighborService service, Neighbor model) =>
        {
            var result = await service.UpdateNeighborAsync(model);
            return Results.Ok(result);
        }).WithName("UpdateNeighbor")
          .WithTags("Neighbors")
          .WithOpenApi(operation => 
            {
                operation.Description = "Endpoint that updates an specific neighbor.";
                return operation;
            })
            .RequireAuthorization("AdminPolicy");

#endregion

#region "Fees Endpoints"
app.MapGet("/fees", async (IFeeService service) => await service.GetFeesAsync())
            .WithName("GetFees")
            .WithTags("Fees")
            .WithOpenApi(operation => 
            {
                operation.Description = "Endpoint that returns the list of feed.";
                return operation;
            })
            .RequireAuthorization("AdminPolicy");

app.MapPost("/fees", async (IFeeService service, Fee model) =>
        {
            var result = await service.CreateFeeAsync(model);
            return Results.Created($"/fees/{result.Id}", result);
        }).WithName("CreateFee")
          .WithTags("Fees")
          .WithOpenApi(operation => 
            {
                operation.Description = "Endpoint that creates a new fee.";
                return operation;
            })
            .RequireAuthorization("AdminPolicy");

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
            })
            .RequireAuthorization("AdminPolicy");

app.MapPut("/fees", async (IFeeService service, Fee model) =>
        {
            var result = await service.UpdateFeeAsync(model);
            return Results.Ok(result);
        }).WithName("UpdateFee")
          .WithTags("Fees")
          .WithOpenApi(operation => 
            {
                operation.Description = "Endpoint that updates an specific fee.";
                return operation;
            })
            .RequireAuthorization("AdminPolicy");

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
            })
          .RequireAuthorization("AdminPolicy");


#endregion

#region "Payments Endpoints"

app.MapGet("/payments", async (IPaymentService service) => await service.GetPaymentsAsync())
                 .WithName("GetPayments")
                 .WithTags("Payments")
                 .WithOpenApi(operation => 
                    {
                        operation.Description = "Endpoint that returns the list of payments.";
                        return operation;
                    })
                    .RequireAuthorization(policy => policy.RequireRole("Admin", "User"));

app.MapPost("/payments", async (IPaymentService service, Payment model) =>    
        {
            var result = await service.CreatePaymentAsync(model);
            return Results.Created($"/payments/{result.Id}", result);
        }).WithName("CreatePayment")
          .WithTags("Payments")
          .WithOpenApi(operation => 
                {
                    operation.Description = "Endpoint that creates a new payment.";
                    return operation;
                })
                .RequireAuthorization(policy => policy.RequireRole("Admin", "User"));

app.MapGet("/payments/neighbors/{id}", async (IPaymentService service, int id) =>
        {
            var result = await service.GetPaymentsByNeighborIdAsync(id);
            return Results.Ok(result);
        }).WithName("GetPaymentsByNeighborId")
          .WithTags("Payments")
          .WithOpenApi(operation => 
            {
                operation.Description = "Endpoint that returns the list of payments by neighbor Id.";
                return operation;
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin", "User"));

app.MapGet("/payments/fees/{id}", async (IPaymentService service, int id) =>
        {
            var result = await service.GetPaymentsByFeeIdAsync(id);
            return Results.Ok(result);
        }).WithName("GetPaymentsByFeeId")
        .WithTags("Payments")
        .WithOpenApi(operation => 
        {
            operation.Description = "Endpoint that returns the list of payments by fee Id.";
            return operation;
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin", "User"));                

app.MapGet("/payments/date/{dateTime}", async (IPaymentService service, DateTime dateTime) =>
        {
            var result = await service.GetPaymentsByDateAsync(dateTime);
            return Results.Ok(result);
        }).WithName("GetPaymentsByDate")
          .WithTags("Payments")
          .WithOpenApi(operation => 
            {
                operation.Description = "Endpoint that returns the list of payments by date.";
                return operation;
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin", "User"));

#endregion

#region "Outcomes Endpoints"

app.MapGet("/outcomes", async (IOutcomeService service) => await service.GetOutcomesAsync())
                 .WithName("GetOutcomes")
                 .WithTags("Outcomes")
                 .WithOpenApi(operation => 
                    {
                        operation.Description = "Endpoint that returns the list of outcomes.";
                        return operation;
                    })
                    .RequireAuthorization(policy => policy.RequireRole("AdminPolicy", "UserPolicy"));

 app.MapPost("/outcomes", async (IOutcomeService service, Outcome model) =>    
        {
            var result = await service.CreateOutcomeAsync(model);
            return Results.Created($"/outcomes/{result.Id}", result);
        }).WithName("CreateOutcome")
          .WithTags("Outcomes")
          .WithOpenApi(operation => 
            {
                operation.Description = "Endpoint that creates a new outcome.";
                return operation;
            })
            .RequireAuthorization(policy => policy.RequireRole("AdminPolicy", "UserPolicy"));                   

app.MapGet("/outcomes/year/{year}", async (IOutcomeService service, string year) =>
        {
            var result = await service.GetOutcomesByYearAsync(year);
            return Results.Ok(result);
        }).WithName("GetOutcomesByYear")
        .WithTags("Outcomes")
        .WithOpenApi(operation => 
        {
            operation.Description = "Endpoint that returns the list of outcomes by year.";
            return operation;
        })
        .RequireAuthorization(policy => policy.RequireRole("AdminPolicy", "UserPolicy"));

app.MapGet("/outcomes/month/{month}", async (IOutcomeService service, string month) =>
        {
            var result = await service.GetOutcomesByMonthAsync(month);
            return Results.Ok(result);
        }).WithName("GetOutcomesByMonth")
        .WithTags("Outcomes")
        .WithOpenApi(operation => 
        {
            operation.Description = "Endpoint that returns the list of outcomes by month.";
            return operation;
        })
        .RequireAuthorization(policy => policy.RequireRole("AdminPolicy", "UserPolicy"));

#endregion

#region "Balances Endpoints"

app.MapGet("/balances", async (IBalanceService service) => await service.GetBalancesAsync())
                 .WithName("GetBalances")
                 .WithTags("Balances")
                 .WithOpenApi(operation => 
                    {
                        operation.Description = "Endpoint that returns the list of balances.";
                        return operation;
                    })
                    .RequireAuthorization(policy => policy.RequireRole("Admin", "User"));

app.MapPost("/balances", async (IBalanceService service, Balance model) =>    
        {
            var result = await service.CreateBalanceAsync(model);
            return Results.Created($"/balances/{result.Id}", result);
        }).WithName("CreateBalance")
          .WithTags("Balances")
          .WithOpenApi(operation => 
            {
                operation.Description = "Endpoint that creates a new balance.";
                return operation;
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin", "User"));

app.MapGet("/balances/year/{year}", async (IBalanceService service, string year) =>
        {
            var result = await service.GetBalancesByYearAsync(year);
            return Results.Ok(result);
        }).WithName("GetBalancesByYear")
        .WithTags("Balances")
        .WithOpenApi(operation => 
        {
            operation.Description = "Endpoint that returns the list of balances by year.";
            return operation;
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin", "User"));

app.MapGet("/balances/period/{period}", async (IBalanceService service, string period) =>
        {
            var result = await service.GetBalancesByPeriodAsync(period);
            return Results.Ok(result);
        }).WithName("GetBalancesByPeriod")
        .WithTags("Balances")
        .WithOpenApi(operation => 
        {
            operation.Description = "Endpoint that returns the list of balances by period.";
            return operation;
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin", "User"));

#endregion

/// <summary>
/// Method to generate a JWT token.
/// </summary>
/// <param name="username">value to create Claim.</param>
/// <returns>JWT token.</returns>
string GenerateJwtToken(string username, string level)
{
    var claims = new[]
    {
        new Claim(ClaimTypes.Name, username),
        new Claim(ClaimTypes.Role, level) 
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: issuer,
        audience: audience,
        claims: claims,
        expires: DateTime.Now.AddHours(1),
        signingCredentials: creds);

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.Run();
