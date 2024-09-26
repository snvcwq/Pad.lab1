using System.Net.Sockets;

namespace BrokerServer;

public static class Topics
{
    private static readonly Dictionary<string, List<Socket>> _topics = [];

    public static void Add(string topicName, Socket socket)
    {
        if(_topics.TryGetValue(topicName, out var topic))
            topic.Add(socket);
        else
            _topics.Add(topicName, [socket]);
    }

    public static List<Socket> GetTopicClients(string topicName) => _topics[topicName];
}