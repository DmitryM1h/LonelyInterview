using LonelyInterviewAudioSession;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LonelyInterview.LLMIntegration;

public class LLMClientFactory(IServiceProvider serviceProvider)
{
    private ConcurrentDictionary<string, LLMClient> _sessions = new();

    public LLMClient GetOrCreateSession(string userId)
    {
        var t = _sessions.TryGetValue(userId, out var session);

        if (t && session is not null)
            return session;

        var scope = serviceProvider.CreateScope();

        var client = scope.ServiceProvider.GetRequiredService<LLMClient>();

        client.SetConnection(userId, CancellationToken.None);
        _sessions[userId] = client;
        return client;
    }

}
