using Grpc.Core;

namespace gRPCClient;

public class PubSubService(PubSub.PubSubClient client)
{

    public async Task RegisterClientAsync()
    {
        var registration = new ClientRegistration();
        registration.SetMessageTopics()
            .AddClientIdentifier();
        var response = await client.RegisterClientAsync(registration);
        Console.WriteLine(response.Messages);
    }

    public async Task StartSendingMessagesAsync()
    {
        using var call = client.SendMessageToTopic();

        _ = Task.Run(async () =>
        {
            await foreach (var message in call.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"Received message on topic {message.Topic}: {message.Messages}");
            }
        });

        while (true)
        {
            var message = new TopicMessage();
            message.SetMessageContent()
                .AddClientTopics()
                .AddClientIdentifier();
            await call.RequestStream.WriteAsync(message);
        }
    }
    
}