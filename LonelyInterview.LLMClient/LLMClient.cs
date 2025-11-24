using Google.Protobuf;
using Grpc.Core;
using LonelyInterviewAudioSession;
using Microsoft.Extensions.Logging;
using System.Runtime.ExceptionServices;
using System.Threading.Channels;

namespace LonelyInterview.LLMIntegration;

public class LLMClient : IAsyncDisposable
{
    private readonly AudioSession.AudioSessionClient _client;
    private readonly ILogger<LLMClient> _logger;
    private const int BUFFER_SIZE = 50;

    private readonly Channel<byte[]> _incomingSpeech = Channel.CreateBounded<byte[]>(BUFFER_SIZE);
    private readonly Channel<byte[]> _modelReplies = Channel.CreateBounded<byte[]>(BUFFER_SIZE);
    private Exception? _exception;
    private string _candidateId = null!;
    private AsyncDuplexStreamingCall<AudioChunkRequest, AudioChunkResponse>? _currentCall;

    public LLMClient(AudioSession.AudioSessionClient client, ILogger<LLMClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public IAsyncEnumerable<byte[]> GetModelReplies() => _modelReplies.Reader.ReadAllAsync();

    public void SetConnection(string userId, CancellationToken token)
    {
        _candidateId = userId;
        _ = RunBackground(token);
    }

    private Task RunBackground(CancellationToken token)
    {
        return Task.Run(async () =>
        {
            try
            {
                await StreamAudioAsync(token);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }, token);
    }

    public async Task ReceiveAudioAsync(byte[] data, CancellationToken token)
    {
        ThrowIfExceptionHappened();
        await _incomingSpeech.Writer.WaitToWriteAsync(token);
        await _incomingSpeech.Writer.WriteAsync(data, token);
    }

    public async Task ReceiveCandidatesCodeAsync(string code, string candId, CancellationToken token)
    {
        try
        {
            await _client.SubmitCodeAsync(new CodeSubmissionRequest() { CandidateId = candId, Code = code }, cancellationToken: token);
        }
        catch (Exception ex)
        {
            _logger.LogError("Ошибка при попытке отправить решение кандидата\n" + ex.Message);
            throw;
        }
    }

    private async Task StreamAudioAsync(CancellationToken token)
    {
        _currentCall = _client.DataStream(cancellationToken: token);

        var readTask = Task.Run(async () =>
        {
            await foreach (var response in _currentCall.ResponseStream.ReadAllAsync(token))
            {
                Console.WriteLine($"🔊 Received audio response from server for candidate: {response.CandidateId}");
                byte[] audioBytes = response.AudioData.ToByteArray();
                await _modelReplies.Writer.WriteAsync(audioBytes, token);
            }
        }, token);

        await _incomingSpeech.Reader.WaitToReadAsync(token);

        await foreach (var message in _incomingSpeech.Reader.ReadAllAsync(token))
        {
            Console.WriteLine("отправлено LLM агенту!");

            await _currentCall.RequestStream.WriteAsync(new AudioChunkRequest
            {
                CandidateId = _candidateId,
                AudioData = ByteString.CopyFrom(message)
            });
        }

        await _currentCall.RequestStream.CompleteAsync();
        await readTask;
    }

    private void ThrowIfExceptionHappened()
    {
        if (_exception is not null)
        {
            _logger.LogError("Ошибка фоновой задачи LLMClient: " + _exception.Message);
            ExceptionDispatchInfo.Throw(_exception);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _currentCall?.Dispose();
        _incomingSpeech.Writer.Complete();
        _modelReplies.Writer.Complete();

        await Task.CompletedTask;
    }


}