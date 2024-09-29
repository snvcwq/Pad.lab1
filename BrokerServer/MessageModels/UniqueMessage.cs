using System.Collections.Concurrent;
using Common.Models;

namespace BrokerServer.MessageModels;

public static class UniqueMessage
{
    public static ConcurrentDictionary<Guid, MessageResponse> Messages = [];

    public static Guid Add(MessageResponse msg)
    {
        var guid = new Guid();
        Messages.TryAdd(guid, msg);
        return guid;
    }

    public static MessageResponse? Get(Guid guid)
    {
        Messages.TryGetValue(guid, out var msg);
        return msg;
    }
}