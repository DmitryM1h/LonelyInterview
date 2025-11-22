


using System.Runtime.ExceptionServices;
using System.Threading.Channels;
using LonelyInterview.LLMIntegration;

namespace LonelyInterview.Application.Interview;

public class AudioInterviewSession
{
    private const int BUFFER_SIZE = 50;

    private readonly Channel<byte[]> _incomingSpeech = Channel.CreateBounded<byte[]>(BUFFER_SIZE);

    private string UserId = null!;

    private LLMClient _llmClient;

    public AudioInterviewSession(LLMClient llmClient)
    {
        _llmClient = llmClient;
    }

    public async Task ReceiveData(byte[] data, CancellationToken token)
    {
        ThrowIfExceptionHappened();
        await _incomingSpeech.Writer.WaitToWriteAsync(token); // На случай, если не будем успевать обрабатывать поток данных вовремя
        await _incomingSpeech.Writer.WriteAsync(data, token);

    }

    public async IAsyncEnumerable<byte[]> ModelAnswers()
    {
        await foreach (var reply in _llmClient.GetModelReplies())
        {
            yield return reply;
        }
    }

    public void StartSession(string userId, CancellationToken token)
    {
        UserId = userId;
    }
    
    public void CompleteSession()
    {
        _incomingSpeech.Writer.Complete();
    }


    private void ThrowIfExceptionHappened()
    {
        if (_llmClient.ClientException is not null)
        {
            Console.WriteLine("Ошибка!!" +  _llmClient.ClientException.Message);
            ExceptionDispatchInfo.Throw(_llmClient.ClientException);
        }
    }

}
