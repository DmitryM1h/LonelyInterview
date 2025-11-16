using LonelyInterview.Auth;
using LonelyInterview.Auth.Contracts;
using LonelyInterview.Domain.Interfaces;
using LonelyInterview.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddDatabaseContext(builder.Configuration);
builder.Services.AddDatabaseUserContext(builder.Configuration);
builder.Services.AddDataSources();


builder.Services.Configure<AuthOptions>(
    builder.Configuration.GetSection("AuthOptions"));

builder.Services.AddAuth();

var identityBuilder = builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(opts =>
{
    opts.Password.RequiredLength = 5;
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequireDigit = false;
})
    .AddEntityFrameworkStores<ApplicationUserContext>();


var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.Use(async (ctx, next) =>
{
    var authEnabled = app.Configuration.GetValue<bool>("AuthOptions:Enabled");
    var chosenRole = app.Configuration.GetValue<string>("AuthOptions:Role");
    if (!Enum.TryParse<Role>(chosenRole, true, out Role role))
    {
        ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
        throw new ArgumentException("Invalid role");
    }
    if (authEnabled)
    {
        await next();
        return;
    }

    var claims = new[]
    {
            
            new Claim(ClaimTypes.Role, chosenRole),
            new Claim("email","dima@gmail.com")
        };
    var identity = new ClaimsIdentity(claims, "DisabledAuth");
    ctx.User = new ClaimsPrincipal(identity);

    await next();
});

app.UseAuthorization();

app.UseCors(builder => builder.AllowAnyOrigin());

app.MapControllers();


app.Run();
