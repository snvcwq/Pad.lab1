using System.Collections.Concurrent;

namespace gRPCServer.MessageModels;

public static class UniqueMessage
{
    public static ConcurrentDictionary<Guid, TopicMessageResponse> Messages = [];

    public static Guid Add(TopicMessageResponse msg)
    {
        var guid = new Guid();
        Messages.TryAdd(guid, msg);
        return guid;
    }

    public static TopicMessageResponse? Get(Guid guid)
    {
        Messages.TryGetValue(guid, out var msg);
        return msg;
    }
}