using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<API.data.AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();
builder.Services.AddScoped<API.interfaces.ITokenService, API.Services.TokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenkey = builder.Configuration["TokenKey"] ?? throw new Exception("Token key not found -program.cs");
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(tokenkey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
var app = builder.Build();

app.UseCors(options =>
{
    options.WithOrigins("https://localhost:4200","http://localhost:4200")
           .AllowAnyMethod()
           .AllowAnyHeader();
});
app.UseAuthentication(); //how are u 
app.UseAuthorization(); // are u allowed to do what u are doing
app.MapControllers();

app.Run();
