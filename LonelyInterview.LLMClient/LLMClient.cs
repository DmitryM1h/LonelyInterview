
using Google.Protobuf;
using Grpc.Core;
using LonelyInterviewAudioSession;
using System.Runtime.ExceptionServices;
using System.Threading.Channels;

namespace LonelyInterview.LLMIntegration;

public class LLMClient(AudioSession.AudioSessionClient client) : AudioSession.AudioSessionClient
{
    private const int BUFFER_SIZE = 50;

    private readonly Channel<byte[]> _incomingSpeech = Channel.CreateBounded<byte[]>(BUFFER_SIZE);
  
    private readonly Channel<byte[]> _modelReplies = Channel.CreateBounded<byte[]>(BUFFER_SIZE);

    private Exception? _exception;

    private string candidateId = null!;



    public IAsyncEnumerable<byte[]> GetModelReplies() => _modelReplies.Reader.ReadAllAsync();

    public void SetConnection(string userId, CancellationToken token)
    {
        candidateId = userId;
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
                Console.WriteLine("Background failed");
                _exception = ex;
            }
        }, token);
    }


    public async Task ReceiveAudioAsync(byte[] data, CancellationToken token)
    {
     
        ThrowIfExceptionHappened();
        await _incomingSpeech.Writer.WaitToWriteAsync(token); // На случай, если не будем успевать обрабатывать поток данных вовремя
        await _incomingSpeech.Writer.WriteAsync(data, token);
       
    }

    public async Task StreamAudioAsync(CancellationToken token)
    {

        var call = client.DataStream(cancellationToken: token);

        var readTask = Task.Run(async () =>
        {
            await foreach (var response in call.ResponseStream.ReadAllAsync(token))
            {
                Console.WriteLine($"Server: {response.AudioData}");
                byte[] audioBytes = response.AudioData.ToByteArray();
                await _modelReplies.Writer.WriteAsync(audioBytes);
            }
        }, token);


        await _incomingSpeech.Reader.WaitToReadAsync(token);

        await foreach (var message in _incomingSpeech.Reader.ReadAllAsync(token))
        {
            Console.WriteLine("Звук считан из канала");

            await call.RequestStream.WriteAsync(new AudioChunkRequest { CandidateId = candidateId, AudioData = ByteString.CopyFrom(message) });
        }

        await call.RequestStream.CompleteAsync();
        await readTask;

    }

    private void ThrowIfExceptionHappened()
    {
        if (_exception is not null)
        {
            Console.WriteLine("Ошибка background " + _exception.Message);
            ExceptionDispatchInfo.Throw(_exception);
        }
    }

}
