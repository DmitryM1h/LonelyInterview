

using LonelyInterview.Auth.Contracts;
using LonelyInterview.LLMIntegration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace LonelyInterview.Application.Interview
{
    [Authorize(Roles = nameof(Role.Candidate))]
    public class InterviewHub(LLMClient client, ILogger<InterviewHub> _logger) : Hub
    {
        public async Task StartAudioStream(IAsyncEnumerable<byte[]> audioStream)
        {
            var connectionId = Context.UserIdentifier!;

            client.SetConnection(connectionId, Context.ConnectionAborted);

            try
            {
                await foreach (var audioChunk in audioStream)
                {
                    await client.ReceiveAudioAsync(audioChunk, Context.ConnectionAborted);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка LLMClient " + ex.ToString());

                await Clients.Caller.SendAsync("AudioProcessingDelay", new
                {
                    Message = "Временные задержки в обработке аудио",
                    Severity = "warning",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        public async Task SubmitCode(string code)
        {
            var connectionId = Context.UserIdentifier!;

            try
            {
                await client.ReceiveCandidatesCodeAsync(code, connectionId ,Context.ConnectionAborted);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка LLMClient " + ex.ToString());

                await Clients.Caller.SendAsync("AudioProcessingDelay", new
                {
                    Message = "Временные задержки в обработке аудио",
                    Severity = "warning",
                    Timestamp = DateTime.UtcNow
                });
            }
        }
        

        public async Task ReceiveModelAnswers()
        {
            await foreach (var reply in client.GetModelReplies())
            {
                // Озвучка ????

                string base64Audio = Convert.ToBase64String(reply);
                Console.WriteLine("Ответ модели: " +  reply);
                await Clients.Caller.SendAsync(base64Audio);
            }
        }


    }
}
