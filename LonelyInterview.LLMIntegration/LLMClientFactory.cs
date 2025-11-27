using LonelyInterviewAudioSession;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;


namespace LonelyInterview.LLMIntegration;

public class LLMClientFactory(IServiceProvider serviceProvider)
{
    private ConcurrentDictionary<string, LLMClient> _sessions = new();

    public LLMClient GetOrCreateSession(string userId)
    {
        var sessionExists = _sessions.TryGetValue(userId, out var session);

        if (sessionExists && session is not null)
            return session;

        var scope = serviceProvider.CreateScope();

        var client = scope.ServiceProvider.GetRequiredService<LLMClient>();

        client.SetConnection(userId, CancellationToken.None);
        _sessions[userId] = client;
        return client;
    }

    public async Task CloseSession(string userId)
    {
        var sessionExists = _sessions.TryGetValue(userId, out var session);
        if (sessionExists && session is not null)
        {
            await session.DisposeAsync();
        }
    }

}
