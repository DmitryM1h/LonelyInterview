using LonelyInterview.Application;
using LonelyInterview.Application.Interview;
using LonelyInterview.Auth;
using LonelyInterview.Infrastructure.Data;
using LonelyInterview.Infrastructure.Data.Converters;
using LonelyInterview.LLMIntegration;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);


builder.Services.AddControllers()
     .AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
     }); ;
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
    options.AddPolicy(name: "AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
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

builder.Services.AddLLMClient();

var app = builder.Build();


await AddDefaultRoles();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseCors("AllowSpecificOrigin");


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


async Task AddDefaultRoles()
{
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        string[] roles = new string[] { "Candidate", "HrManager", "Administrator" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole<Guid>(roleName);
                var result = await roleManager.CreateAsync(role);

            }
        }
    }

}

