using FinanceService.DAL;
using FinanceService.BAL;
using FinanceService.Controllers;
using FinanceService.DAL.Interfaces;
using FinanceService.DAL.Repositories;
using FinanceService.BAL.Interfaces;
using FinanceService.BAL.Repositories;
using StocksAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddGrpc();

// Add Dapper Context
builder.Services.AddSingleton<DapperContext>();

// Register repository with ILogger
builder.Services.AddScoped<IStockDAL, StockDAL>();

// Register business logic
builder.Services.AddScoped<IFinanceBAL, FinanceBAL>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapGrpcService<FinanceGrpcService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();