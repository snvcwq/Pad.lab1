using Grpc.Core;

namespace gRPCServer.Services;

public class PubSubSvc: PubSubService.PubSubServiceBase
{
    private readonly Dictionary<string, List<string>> _clientSubscriptions = new();
    private readonly List<IServerStreamWriter<TopicMessage>> _connectedClients = new();

    // Register the client and their topics
    public override Task<RegistrationResponse> RegisterClient(ClientRegistration request, ServerCallContext context)
    {
        _clientSubscriptions[request.ClientId] = request.Topics.ToList();
        return Task.FromResult(new RegistrationResponse
        {
            Success = true,
            Messages = { $"Client {request.ClientId} registered successfully" }
        });
    }

    // Handle sending and receiving topic messages
    public override async Task SendMessageToTopic(IAsyncStreamReader<TopicMessage> requestStream, IServerStreamWriter<TopicMessage> responseStream, ServerCallContext context)
    {
        _connectedClients.Add(responseStream);

        await foreach (var message in requestStream.ReadAllAsync())
        {
            var topic = message.Topic;

            // Check which clients are subscribed to the topic
            var subscribedClients = _clientSubscriptions
                .Where(sub => sub.Value.Contains(topic))
                .Select(sub => sub.Key);

            // Broadcast the message to the clients subscribed to this topic
            foreach (var client in subscribedClients)
            {
                foreach (var clientStream in _connectedClients)
                {
                    // Send the message to all clients that are connected
                    await clientStream.WriteAsync(new TopicMessage
                    {
                        ClientId = message.ClientId,
                        Topic = message.Topic,
                        Message = message.Message
                    });
                }
            }
        }
    }
}