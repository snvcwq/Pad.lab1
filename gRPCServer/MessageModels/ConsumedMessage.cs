using System.Collections.Concurrent;

namespace gRPCServer.MessageModels;

public class ConsumedMessage
{
    public static ConcurrentDictionary<string, List<Guid>> Messages = [];

    public static void Add(string client, Guid msgGuid)
    {
        Messages.TryAdd(client, []);
        Messages[client].Add(msgGuid);
    }

    public static List<TopicMessageResponse?> GetUnconsumedMessages(string client, List<string> topics)
    {
        var result = Messages.TryGetValue(client, out var uniqueMessages);
        var consumedMessages = uniqueMessages != null && result && uniqueMessages.Any() ? uniqueMessages : [];
        var allMessages = new List<Guid>();
        foreach (var topic in topics)
            allMessages.AddRange(Topics.GetMessages(topic));
        return allMessages.ToList().Except(consumedMessages).Select(UniqueMessage.Get).ToList();
    }
}