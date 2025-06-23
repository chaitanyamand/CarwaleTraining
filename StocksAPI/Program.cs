using StocksAPI.BAL;
using StocksAPI.DAL;
using StocksAPI.Mappings;
using StocksAPI.Data;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

// Define CORS policy
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

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configure Dapper to use a custom type handler for List<string>
SqlMapper.AddTypeHandler(new JsonListTypeHandler());

// Register dependencies
builder.Services.AddScoped<IStockDAL, StockDAL>();
builder.Services.AddScoped<IStockBAL, StockBAL>();
builder.Services.AddSingleton<DapperContext>();

var app = builder.Build();

// Enable CORS before any endpoint middleware
app.UseCors(corsPolicyName);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
