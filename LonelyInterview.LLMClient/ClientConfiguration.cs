using LonelyInterviewAudioSession;
using Microsoft.Extensions.DependencyInjection;


namespace LonelyInterview.LLMIntegration;

public static class ClientConfiguration
{
    public static void AddLLMClient(this IServiceCollection services)
    {
        services.AddGrpcClient<AudioSession.AudioSessionClient>(options =>
        {
            options.Address = new Uri("https://python-llm-service:50051");
        });

        services.AddScoped<LLMClient>();
    }

}
