using System.Collections.Concurrent;

namespace gRPCServer.Models;

public static class ClientRegistry
{
    public static ConcurrentDictionary<string, Client> Clients = new();

    public static IEnumerable<Client> GetTopicSubscribers(string topicName)
    {
        return Clients.Values.Where(v => v.Topics.Any(t => t == topicName));
    }
}