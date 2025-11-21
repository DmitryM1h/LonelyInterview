
using Grpc.Core;
using LonelyInterviewAudioSession;
using System.Threading.Channels;
using Google.Protobuf;

namespace LonelyInterview.LLMIntegration;

public class LLMClient(AudioSession.AudioSessionClient client) : AudioSession.AudioSessionClient
{
    private const int BUFFER_SIZE = 50;

    private ChannelReader<byte[]> _incomingSpeech = null!;

    private readonly Channel<byte[]> _modelReplies = Channel.CreateBounded<byte[]>(BUFFER_SIZE);
    public Exception? ClientException { get; private set; }

    public void SetConnection(ChannelReader<byte[]> reader, CancellationToken token)
    {
        _incomingSpeech = reader;

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
                ClientException = ex;
            }
        }, token);
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
            }
        }, token);

        await foreach (var message in _incomingSpeech.ReadAllAsync())
        {
            Console.WriteLine(message);

            await call.RequestStream.WriteAsync(new AudioChunkRequest { CandidateId = "kek", AudioData = ByteString.CopyFrom(message) });
            await Task.Delay(2000, token);
        }

        await call.RequestStream.CompleteAsync();
        await readTask;

    }

    public IAsyncEnumerable<byte[]> GetModelReplies() => _modelReplies.Reader.ReadAllAsync();


}
