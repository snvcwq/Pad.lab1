using CommongRPC;
using Grpc.Core;
using gRPCServer.Helpers;
using gRPCServer.Models;

namespace gRPCServer.Services
{
    public class PubSubSvc : PubSub.PubSubBase
    {

        public override async Task SendMessageToTopic(IAsyncStreamReader<TopicMessage> requestStream, IServerStreamWriter<TopicMessageResponse> responseStream, ServerCallContext context)
        {
            await foreach (var message in requestStream.ReadAllAsync())
            {
                Console.WriteLine($"Got a message from {message.ClientId}");
                if (message.IsRegistrationMessage)
                {
                    var registrationResult = message.HandleRegistrationRequest(responseStream);
                    await responseStream.WriteAsync(registrationResult);
                }
                else
                {
                    var response = new TopicMessageResponse();
                    foreach (var topic in message.Topics)
                    {
                        var subs = ClientRegistry.GetTopicSubscribers(topic);
                        if(!subs.Any())
                            response.Messages.Add($"Topic {topic} has no subscribers".AddBrokerPrefix());
                        else
                        {
                            foreach (var sub in subs)
                            {
                                var msg = new TopicMessageResponse
                                {
                                    Topic = topic,
                                    Messages = { $"You got a message from {message.ClientId} threw topic {topic}".AddBrokerPrefix(), message.Message},
                                    IsSuccessful = true
                                };

                                await sub.Stream.WriteAsync(msg);
                                response.Messages.Add($"Message was sent to {sub.ClientId} threw topic {topic}".AddBrokerPrefix());
                            }
                        }
                    }

                    await responseStream.WriteAsync(response);
                    Console.WriteLine($"Message from {message.ClientId} consumed successfully");
                }
            }
        }

    }
}
