using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<API.data.AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();
   
var app = builder.Build();

app.UseCors(options =>
{
    options.WithOrigins("https://localhost:4200","http://localhost:4200")
           .AllowAnyMethod()
           .AllowAnyHeader();
});
app.MapControllers();

app.Run();
