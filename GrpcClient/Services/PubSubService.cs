using CommongRPC;
using Grpc.Core;
using gRPCClient.Helpers;

namespace gRPCClient.Services;

public static class PubSubService
{
    public static void StartReceivingMessagesAsync(this  AsyncDuplexStreamingCall<TopicMessage,TopicMessageResponse>? call)
    {
        _ = Task.Run(async () =>
        {
            await foreach (var message in call.ResponseStream.ReadAllAsync())
            {
                if (message.IsRegistrationResponse)
                    message.HandleRegistrationResponse();
                else
                    message.HandleMessageResponse();
            }
        });
    }
    
    public static async Task StartSendingMessagesAsync(this  AsyncDuplexStreamingCall<TopicMessage,TopicMessageResponse>? call)
    {

        while (true)
        {
            var message = new TopicMessage();
            if (!ClientData.IsRegistered)
            {
                RegistrationHelper.AddClientIdentifier();
                message
                    .SetRegistrationMethod()
                    .AddClientTopics()
                    .AddClientIdentifier();
            }
            else
            {
                message.SetMessageContent()
                    .SetMessageTopics()
                    .AddClientIdentifier();
            }
            await call.RequestStream.WriteAsync(message);
            Console.WriteLine("Message was sent to Broker".AddInfoPrefix());
            await Task.Delay(10000);
        }
    }
}
