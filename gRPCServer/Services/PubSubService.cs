using System.Collections.Concurrent;
using Grpc.Core;

namespace gRPCServer.Services
{
    public class PubSubSvc : PubSub.PubSubBase
    {
        // Dictionary to store clients and their subscribed topics
        private readonly ConcurrentDictionary<string, List<string>> _clientSubscriptions = new();

        // List to store active client streams for broadcasting messages
        private readonly List<IServerStreamWriter<TopicMessageResponse>> _connectedClients = new();

        // Register the client and their topics
        public override Task<RegistrationResponse> RegisterClient(ClientRegistration request, ServerCallContext context)
        {
            // Store the client's subscribed topics
            _clientSubscriptions[request.ClientId] = request.Topics.ToList();
            return Task.FromResult(new RegistrationResponse
            {
                Success = true,
                Messages = { $"Client {request.ClientId} registered successfully with topics: {string.Join(", ", request.Topics)}" }
            });
        }

        // Handle sending and receiving topic messages
        public override async Task SendMessageToTopic(
            IAsyncStreamReader<TopicMessage> requestStream,
            IServerStreamWriter<TopicMessageResponse> responseStream,
            ServerCallContext context)
        {
            // Add the response stream to the list of connected clients
            _connectedClients.Add(responseStream);

            await foreach (var message in requestStream.ReadAllAsync())
            {
                // Iterate through all topics the message is targeting
                foreach (var topic in message.Topics)
                {
                    // Check which clients are subscribed to the topic
                    var subscribedClients = _clientSubscriptions
                        .Where(sub => sub.Value.Contains(topic))
                        .Select(sub => sub.Key);

                    // Prepare the message to broadcast
                    var response = new TopicMessageResponse
                    {
                        Topic = topic,
                        ClientId = message.ClientId,
                        Messages = { message.Message }
                    };

                    // Broadcast the message to the clients subscribed to this topic
                    foreach (var clientStream in _connectedClients)
                    {
                        // Send the message to all connected clients
                        await clientStream.WriteAsync(response);
                    }
                }
            }
        }
    }
}
