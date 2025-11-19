


using System.Runtime.ExceptionServices;
using System.Threading.Channels;

namespace LonelyInterview.Application.Interview;

public class AudioInterviewSession
{
    private const int BUFFER_SIZE = 50;

    private readonly Channel<string> _incomingSpeech = Channel.CreateBounded<string>(BUFFER_SIZE);

    private string UserId = null!;

    private readonly CancellationTokenSource _CancellationToken;

    private Exception? _Exception;

    public AudioInterviewSession(/*LLMClient llmClient*/)
    {
        _CancellationToken = new CancellationTokenSource();
        _CancellationToken.Token.Register(CompleteSession);
        //_llmClient = llmClient;
    }

    public async Task ReceiveData(string data)
    {
        ThrowIfExceptionHappened();
        await _incomingSpeech.Writer.WaitToWriteAsync(_CancellationToken.Token); // На случай, если не будем успевать обрабатывать поток данных вовремя
        await _incomingSpeech.Writer.WriteAsync(data, _CancellationToken.Token);
    }

    //public async IAsyncEnumerable<string> ModelAnswers()
    //{
    //    await foreach(var reply in _llmClient.GetModelReplies())
    //    {
    //        yield return reply;
    //    }
    //}

    public async Task ProcessDataAsync()
    {
        await foreach (var chunk in _incomingSpeech.Reader.ReadAllAsync(_CancellationToken.Token))
        {
            Console.WriteLine($"Received audio chunk of {chunk.Length} bytes at {DateTime.Now:HH:mm:ss.fff}");

            var audioData = Convert.FromBase64String(chunk);

            // отправка к LLM

        }
    }

    public void CompleteSession()
    {
        _incomingSpeech.Writer.Complete();
    }


    public void StartSession(string userId)
    {
        UserId = userId;
        _ = RunBackground();
        //_llmClient.SetChannel(_incomingSpeech);

        //_ = Task.Run(_llmClient.StreamAudioAsync);

    }


    private Task RunBackground()
    {
        return Task.Run(async () =>
        {
            try
            {
                await ProcessDataAsync();
            }
            catch (Exception ex)
            {
                await _CancellationToken.CancelAsync();
                _Exception = ex;
            }
        });
    }

    private void ThrowIfExceptionHappened()
    {
        if (_Exception is not null)
            ExceptionDispatchInfo.Throw(_Exception);
    }

}
