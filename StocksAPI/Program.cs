using StocksAPI.Mappings;
using StocksAPI.Data;
using Dapper;
using StocksAPI.DAL.Interfaces;
using StocksAPI.BAL.Interfaces;
using StocksAPI.BAL.Services;
using StocksAPI.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Define CORS policy to allow requests from localhost:5173 
var corsPolicyName = "AllowLocalhost5173";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName, policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add controller support
builder.Services.AddControllers();

// Enable API documentation (Swagger/OpenAPI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register AutoMapper with the mapping profile
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configure gRPC client for the Finance service
builder.Services.AddGrpcClient<FinanceService.Protos.Finance.FinanceClient>(o =>
{
    o.Address = new Uri("http://localhost:5021");
});

// Register custom Dapper type handler for List<string> stored as JSON
SqlMapper.AddTypeHandler(new JsonListTypeHandler());

// Register data access and business logic layer dependencies
builder.Services.AddScoped<IStockDAL, StockDAL>();
builder.Services.AddScoped<IStockBAL, StockBAL>();

// Register Dapper database context as a singleton
builder.Services.AddSingleton<DapperContext>();

var app = builder.Build();

// Enable CORS middleware with the defined policy
app.UseCors(corsPolicyName);

// Enable Swagger UI in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Map controller endpoints
app.MapControllers();

// Start the application
app.Run();
