using FastEndpoints;
using FastEndpoints.Security;
using Jelly;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();

builder.Services.Configure<SecretConfig>(builder.Configuration.GetSection("SecretConfig"));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:5147")  // Specify the allowed origin
              .AllowAnyHeader()                    // Allow any headers
              .AllowAnyMethod();                   // Allow any HTTP method (GET, POST, etc.)
    });
});
builder.Services.AddDbContext<JellyDB>(options => options.UseSqlite("Data Source=Jelly.db"));

string SigningKey = builder.Configuration["SecretConfig:Key"]!;
builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = SigningKey)
   .AddAuthorization()
   .AddFastEndpoints();

var app = builder.Build();

app.UseCors("AllowLocalhost");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<JellyDB>();
    dbContext.ConstructDBFromData();
}

app.UseAuthentication()
   .UseAuthorization()
   .UseFastEndpoints();

app.Run();

//rm Migrations
//dotnet ef migrations add InitialCreate
//dotnet ef database update