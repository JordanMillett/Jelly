global using FastEndpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();

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

builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseCors("AllowLocalhost");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<JellyDB>();
    dbContext.ConstructDBFromData();
}

app.UseFastEndpoints();
app.Run();

//dotnet ef migrations add InitialCreate
//dotnet ef database update