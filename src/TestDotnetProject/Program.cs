using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using TestDotnetProject.Application;
using TestDotnetProject.Domain;
using TestDotnetProject.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
        };
    });

builder.Services.AddAuthorization();

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddControllers();

builder.Services.AddDbContext<UsersDbContext>(options => options.UseNpgsql(
    Environment.GetEnvironmentVariable("NPGSQL_CONNECTION_STRING")));

builder.Services.AddScoped<UsersRepository, UsersRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(document =>
    {
        document.AddSecurity("Bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT",
            Description = "Type into the textbox: {your JWT token}."
        });

        document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
    await db.Database.MigrateAsync();

    var adminUserLogin = Environment.GetEnvironmentVariable("ADMIN_USER_LOGIN")
        ?? throw new InvalidOperationException("Environment variable 'ADMIN_USER_LOGIN' must be set");

    var adminUser = await db.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Login == adminUserLogin);

    if (adminUser is null)
    {
        var adminUserPassword = Environment.GetEnvironmentVariable("ADMIN_USER_PASSWORD")
            ?? throw new InvalidOperationException("Environment variable 'ADMIN_USER_PASSWORD' must be set");

        db.Users.Add(new User(
            adminUserLogin,
            adminUserPassword,
            "SystemAdmin",
            2,
            null,
            true,
            "SystemAdmin"));
        
        await db.SaveChangesAsync();
    }
                    
}

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
