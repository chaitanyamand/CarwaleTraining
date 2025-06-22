using StocksAPI.BAL;
using StocksAPI.DAL;
using StocksAPI.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register dependencies
builder.Services.AddScoped<IStockDAL, StockDAL>();
builder.Services.AddScoped<IStockBAL, StockBAL>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();