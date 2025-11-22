


using LonelyInterview.Auth.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LonelyInterview.Application.Interview
{
    [Authorize(Roles = nameof(Role.Candidate))]
    public class InterviewHub(AudioInterviewSession session) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Соединение установлено: {Context.ConnectionId}");

            var connectionId = Context.UserIdentifier!;
            session.StartSession(connectionId, Context.ConnectionAborted);

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Соединение разорвано: {Context.ConnectionId}");
            
            return base.OnDisconnectedAsync(exception);
        }

        public async Task StartAudioStream(IAsyncEnumerable<byte[]> audioStream)
        {

            await foreach (var audioChunk in audioStream)
            {
                //Console.WriteLine("Received in hub endpoint");
               // Console.WriteLine($"{audioChunk}");
                //TODO отлавливание ошибок и retry + оповещение кандидата о том, что система тормозит
                
                await session.ReceiveData(audioChunk, Context.ConnectionAborted);


            }
            Console.WriteLine("Audio stream completed");
        }

        public async Task ReceiveModelAnswers()
        {
            await foreach (var reply in session.ModelAnswers())
            {
                // Озвучка ????

                string base64Audio = Convert.ToBase64String(reply);
                await Clients.Caller.SendAsync(base64Audio);
            }
        }


    }
}
