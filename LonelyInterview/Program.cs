using LonelyInterview.Application;
using LonelyInterview.Application.Interview;
using LonelyInterview.Auth;
using LonelyInterview.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Token in format: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddDatabaseContext(builder.Configuration);
builder.Services.AddDatabaseUserContext(builder.Configuration);
builder.Services.AddDataSources();

builder.Services.Configure<AuthOptions>(
    builder.Configuration.GetSection("AuthOptions"));


var identityBuilder = builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(opts =>
{
    opts.Password.RequiredLength = 5;
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequireDigit = false;
})
    .AddEntityFrameworkStores<ApplicationUserContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuth(builder.Configuration); // Добавляем после identity чтобы перезатереть дефолтную cookie авторизацию

builder.Services.AddApplicationServices();



var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//app.Use(async (ctx, next) =>
//{
//    Console.WriteLine($"➡️ Incoming Request: {ctx.Request.Method} {ctx.Request.Path}");
//    Console.WriteLine($"Authorization Header: {ctx.Request.Headers.Authorization}");

//    await next();

//    Console.WriteLine($"⬅️ Response: {ctx.Response.StatusCode}");
//});

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseCors("AllowAll");


app.UseRouting();


app.UseAuthentication();

// app.Use(async (ctx, next) =>
// {
//     var authEnabled = app.Configuration.GetValue<bool>("AuthOptions:Enabled");
//     var chosenRole = app.Configuration.GetValue<string>("AuthOptions:Role");
//     if (!Enum.TryParse<Role>(chosenRole, true, out Role role))
//     {
//         ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
//         throw new ArgumentException("Invalid role");
//     }
//     if (authEnabled)
//     {
//         try
//         {
//             await next();
//         }
//         catch(Exception ex)
//         {
//             Console.WriteLine(ex.ToString());
//             throw;
//         }
//         return;
//     }
//
//     var claims = new[]
//     {
//
//             new Claim(ClaimTypes.Role, chosenRole),
//             new Claim("email","dima@gmail.com")
//         };
//     var identity = new ClaimsIdentity(claims, "DisabledAuth");
//     ctx.User = new ClaimsPrincipal(identity);
//
//     await next();
// });



app.UseAuthorization();


app.MapHub<InterviewHub>("/interview");
app.MapControllers();


app.Run();
