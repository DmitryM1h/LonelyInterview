

using LonelyInterview.Auth.Contracts;
using LonelyInterview.LLMIntegration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LonelyInterview.Application.Interview
{
    [Authorize(Roles = nameof(Role.Candidate))]
    public class InterviewHub(LLMClient client) : Hub
    {
 
    
        public async Task StartAudioStream(IAsyncEnumerable<byte[]> audioStream)
        {
            var connectionId = Context.UserIdentifier!;

            client.SetConnection(connectionId, Context.ConnectionAborted);

            await foreach (var audioChunk in audioStream)
            {
                //Console.WriteLine("Received in hub endpoint");
                // Console.WriteLine($"{audioChunk}");
                //TODO отлавливание ошибок и retry + оповещение кандидата о том, что система тормозит

                await client.ReceiveAudioAsync(audioChunk, Context.ConnectionAborted);


            }
        }

        public async Task ReceiveModelAnswers()
        {
            await foreach (var reply in client.GetModelReplies())
            {
                // Озвучка ????

                string base64Audio = Convert.ToBase64String(reply);
                await Clients.Caller.SendAsync(base64Audio);
            }
        }


    }
}
