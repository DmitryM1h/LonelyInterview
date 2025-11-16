using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LonelyInterview.Auth
{
    public static class AuthConfiguration
    {
        public static void AddAuth(this IServiceCollection services)
        {
            //var serviceProvider = services.BuildServiceProvider();
            //var jwtOptions = serviceProvider.GetRequiredService<IOptions<AuthOptions>>().Value;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies["JWT"];
                            return Task.CompletedTask;
                        }
                    };
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "Dmih",
                        ValidateAudience = true,
                        ValidAudience = "Noone",
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mysupersecret_secretsecretsecretkey!123"))

                    };
                });

            services.AddScoped<TokenGenerator>();

        }
    }
}
