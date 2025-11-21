using LonelyInterview.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using LonelyInterview.Application.Interview;

namespace LonelyInterview.Application
{
    public static class ConfigureServices
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<AuthService>();
            services.AddSignalR();
            services.AddScoped<AudioInterviewSession>();

        }
          
        
    }
}
