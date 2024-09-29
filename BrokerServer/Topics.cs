using System.Collections.Concurrent;
using System.Net.Sockets;

namespace BrokerServer;

public static class Topics
{
    private static readonly ConcurrentDictionary<string, List<(Socket, string)>> TopicsList = [];
    private static readonly ConcurrentDictionary<string, List<Guid>> TopicMessages = [];

    public static void AddMessage(string topicName, Guid msgGuid)
    {
        var result = TopicMessages.TryGetValue(topicName, out var msges);
        if (result)
            msges?.Add(msgGuid);
        else
            TopicMessages.TryAdd(topicName, [msgGuid]);
    }

    public static List<Guid> GetMessages(string topicName)
    {
        if (TopicMessages.TryGetValue(topicName, out var messages))
            return messages;
        TopicMessages.TryAdd(topicName, []);
        return [];
    }

    public static void Add(string topicName, (Socket, string) socket)
    {
        if (TopicsList.TryGetValue(topicName, out var topic))
            topic.Add(socket);
        else
            TopicsList.TryAdd(topicName, [socket]);
    }

    public static List<(Socket, string)> GetTopicClients(string topicName)
    {
        return TopicsList[topicName];
    }
}