using LonelyInterview.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
