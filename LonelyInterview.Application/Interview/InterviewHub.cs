


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

            var connectionId = Context.UserIdentifier ?? Context.ConnectionId;
            
            session.StartSession(connectionId);

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Соединение разорвано: {Context.ConnectionId}");

            session.CompleteSession();
            
            return base.OnDisconnectedAsync(exception);
        }

        public async Task StartAudioStream(IAsyncEnumerable<string> audioStream)
        {

            await foreach (var audioChunk in audioStream)
            {
                Console.WriteLine("Received in hub endpoint");
                //TODO отлавливание ошибок и retry + оповещение кандидата о том, что система тормозит
                
                await session.ReceiveData(audioChunk);

            }
            Console.WriteLine("Audio stream completed");
        }

        public async Task ReceiveModelAnswers()
        {
            //await foreach (var reply in userSession.ModelAnswers())
            //{
            //    // Озвучка ????

            //    await Clients.Caller.SendAsync(reply);
            //}
        }


    }
}
