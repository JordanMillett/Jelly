global using FastEndpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();

builder.Services.AddDbContext<JellyDB>(options => options.UseSqlite("Data Source=Jelly.db"));

builder.Services.AddFastEndpoints();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<JellyDB>();
    dbContext.ConstructDBFromData();
}


app.UseFastEndpoints();
app.Run();

//dotnet ef migrations add InitialCreate
//dotnet ef database update