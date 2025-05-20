using Microsoft.EntityFrameworkCore;
using TestDotnetProject;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddControllers();

builder.Services.AddDbContext<UsersDbContext>(options => options.UseInMemoryDatabase("Users"));

builder.Services.AddScoped<UsersRepository, UsersRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();
