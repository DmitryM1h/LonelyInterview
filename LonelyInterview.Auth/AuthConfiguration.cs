using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LonelyInterview.Auth
{
    public static class AuthConfiguration
    {
        public static void AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            //var serviceProvider = services.BuildServiceProvider();
            //var jwtOptions = serviceProvider.GetRequiredService<IOptions<AuthOptions>>().Value;
            var authOptions = configuration.GetSection("AuthOptions").Get<AuthOptions>();


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    //options.Events = new JwtBearerEvents()
                    //{
                    //    OnMessageReceived = context =>
                    //    {
                    //        context.Token = context.Request.Cookies["JWT"];
                    //        return Task.CompletedTask;
                    //    }
                    //};

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            Console.WriteLine("Token validated successfully");
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                            return Task.CompletedTask;
                        },
                        OnMessageReceived = context =>
                        {
                            //var accessToken = context.Request.Headers["Authorization"].FirstOrDefault();
                            //if (!string.IsNullOrEmpty(accessToken) && accessToken.StartsWith("Bearer "))
                            //{
                            //    context.Token = accessToken.Substring("Bearer ".Length);
                            //    Console.WriteLine($"✅ JWT token extracted from Authorization header");
                            //    return Task.CompletedTask;
                            //}

                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/interview"))
                            {
                                context.Token = accessToken;
                                Console.WriteLine($"JWT token extracted from query string for SignalR");
                            }

                            return Task.CompletedTask;
                        },
                    };
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = authOptions!.Issuer,
                        ValidateAudience = true,
                        ValidAudience = authOptions.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.Key))

                    };
                });

            services.AddScoped<TokenGenerator>();

        }
    }
}
