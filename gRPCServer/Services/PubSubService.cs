using CommongRPC;
using Grpc.Core;
using gRPCServer.Helpers;
using gRPCServer.MessageModels;
using gRPCServer.Models;

namespace gRPCServer.Services;

public class PubSubSvc : PubSub.PubSubBase
{
    public override async Task SendMessageToTopic(IAsyncStreamReader<TopicMessage> requestStream,
        IServerStreamWriter<TopicMessageResponse> responseStream, ServerCallContext context)
    {
        await foreach (var message in requestStream.ReadAllAsync())
        {
            Console.WriteLine($"Start Processing Message from {message.ClientId}");
            if (message.IsRegistrationMessage)
            {
                var registrationResult = message.HandleRegistrationRequest(responseStream);
                var unconsumedMessages =
                    ConsumedMessage.GetUnconsumedMessages(message.ClientId, message.Topics.ToList());
                foreach (var msg in unconsumedMessages)
                    await responseStream.WriteAsync(msg);
                await responseStream.WriteAsync(registrationResult);
            }
            else
            {
                var response = new TopicMessageResponse();
                foreach (var topic in message.Topics)
                {
                    var msg = new TopicMessageResponse
                    {
                        Topic = topic,
                        Messages =
                        {
                            $"You got a message from {message.ClientId} using topic {topic}".AddBrokerPrefix(),
                            message.Message
                        },
                        IsSuccessful = true
                    };
                    var msgGuid = UniqueMessage.Add(msg);

                    var subs = ClientRegistry.GetTopicSubscribers(topic);
                    if (!subs.Any())
                        response.Messages.Add($"Topic {topic} has no subscribers".AddBrokerPrefix());
                    else
                        foreach (var sub in subs)
                        {
                            await sub.Stream.WriteAsync(msg);
                            ConsumedMessage.Add(sub.ClientId, msgGuid);

                            response.Messages.Add($"Message was sent to {sub.ClientId} using topic {topic}"
                                .AddBrokerPrefix());
                        }

                    Topics.AddMessage(topic, msgGuid);
                }

                await responseStream.WriteAsync(response);
                Console.WriteLine($"Message from {message.ClientId} consumed successfully");
            }
        }
    }
}