using LonelyInterview.Application;
using LonelyInterview.Application.Interview;
using LonelyInterview.Auth;
using LonelyInterview.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore; 

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });

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
            builder.WithOrigins("http://localhost:5173") // фронтенд URL
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials(); //  куки
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

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>(); // Используем ILogger<Program> или ILogger<Startup>

    // Список ролей, которые мы хотим гарантировать
    string[] roles = new string[] { "Candidate" , "HrManager" /*"Admin", "Interviewer" */ };

    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var role = new IdentityRole<Guid>(roleName);
            var result = await roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                logger.LogInformation($"Role '{roleName}' created successfully.");
            }
            else
            {
                logger.LogError($"Error creating role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            logger.LogInformation($"Role '{roleName}' already exists.");
        }
    }
}


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
app.UseAuthorization();

app.MapHub<InterviewHub>("/interview");
app.MapControllers();

app.Run();

// Отдельный класс для конвертации DateOnly
public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string Format = "yyyy-MM-dd";
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateOnly.ParseExact(reader.GetString()!, Format, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
    }
}