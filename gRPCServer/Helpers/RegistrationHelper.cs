using CommongRPC;
using Grpc.Core;
using gRPCServer.Models;

namespace gRPCServer.Helpers;

public static class RegistrationHelper
{
    public static TopicMessageResponse HandleRegistrationRequest(this TopicMessage message, IServerStreamWriter<TopicMessageResponse> responseStream)
    {
        var response = new TopicMessageResponse
        {
            IsRegistrationResponse = true,
            ClientId = message.ClientId
        };
        var client = new Client
        {
            ClientId = message.ClientId,
            Topics = message.Topics.ToList(),
            Stream = responseStream
        };
        var registrationResult = ClientRegistry.Clients.TryAdd(message.ClientId, client);
        if (registrationResult)
        {
            response.IsSuccessful = true;
            response.Messages.Add($"Registration ended successfully,client subscribed to all topics".AddBrokerPrefix());
        }
        else
        {
            response.IsSuccessful = false;
            response.Messages.Add($"Registration failed. try other Identifier".AddBrokerPrefix());
        }
        return response;
    }
}