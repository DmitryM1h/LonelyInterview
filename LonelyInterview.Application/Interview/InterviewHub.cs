




using Microsoft.AspNetCore.SignalR;

namespace LonelyInterview.Application.Interview
{
    public class InterviewHub : Hub
    {

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Соединение установлено: {Context.ConnectionId}");

            var connectionId = Context.UserIdentifier ?? Context.ConnectionId;

            //userSession.StartSession(connectionId);

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Соединение разорвано: {Context.ConnectionId}");

           // userSession.CompleteSession();

            return base.OnDisconnectedAsync(exception);
        }

        public async Task StartAudioStream(IAsyncEnumerable<string> audioStream)
        {

            await foreach (var audioChunk in audioStream)
            {
                Console.WriteLine(audioChunk);
                //TODO отлавливание ошибок и retry + оповещение кандидата о том, что система тормозит
                //await userSession.ReceiveData(audioChunk);

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
