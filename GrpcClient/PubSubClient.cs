using gRPCServer;
/*
namespace gRPCClient;

public class PubSubClient(PubSubService.PubSubServiceBase client)
{
    private readonly PubSubService.PubSubServiceClient _client = client;

    // Register the client with its ID and the topics it subscribes to
    public async Task RegisterClientAsync(string clientId, List<string> topics)
    {
        var registration = new ClientRegistration
        {
            ClientId = clientId,
            Topics = { topics }
        };

        var response = await _client.RegisterClientAsync(registration);
        Console.WriteLine(response.Message);
    }

    // Start sending messages continuously to the given topic
    public async Task StartSendingMessagesAsync(string topic, string clientId)
    {
        using var call = _client.SendMessageToTopic();

        // Start a task to read incoming messages from the server
        _ = Task.Run(async () =>
        {
            await foreach (var message in call.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"Received message on topic {message.Topic}: {message.Message}");
            }
        });

        // Continuously send messages to the specified topic
        while (true)
        {
            var message = new TopicMessage
            {
                Topic = topic,
                ClientId = clientId,
                Message = $"Message from {clientId} at {DateTime.Now}"
            };

            await call.RequestStream.WriteAsync(message);
            await Task.Delay(1000); // Send a message every second
        }
    }
}*/