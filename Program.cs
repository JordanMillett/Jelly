global using FastEndpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();

builder.Services.AddDbContext<JellyDB>(options => options.UseSqlite("Data Source=Jelly.db"));

builder.Services.AddFastEndpoints();

var app = builder.Build();
app.UseFastEndpoints();
app.Run();

//dotnet ef migrations add InitialCreate
//dotnet ef database update