using StudentPortal.Middleware;
using StudentPortal.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Repository Pattern
builder.Services.AddSingleton<IStudentRepository, StudentRepository>();

var app = builder.Build();

// Custom Middleware
app.UseSimpleLogging();

app.UseStaticFiles();

app.UseRouting();

// MVC conventional routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Student}/{action=Index}/{id?}");

app.Run();
