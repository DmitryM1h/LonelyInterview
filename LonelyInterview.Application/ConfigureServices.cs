using LonelyInterview.Application.Interview;
using LonelyInterview.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LonelyInterview.Application
{
    public static class ConfigureServices
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<AuthService>();
            services.AddSignalR(options =>
            {
                options.MaximumReceiveMessageSize = 1024 * 1024;
                options.StreamBufferCapacity = 1024;
            });
            services.AddScoped<AudioInterviewSession>();

        }
          
        
    }
}
