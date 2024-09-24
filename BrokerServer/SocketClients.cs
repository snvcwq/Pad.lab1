using System.Net.Sockets;

namespace BrokerServer;

public static class SocketClients
{
    private static readonly Dictionary<string, Socket> Clients = new();

    /// <summary>
    ///     Add Socket to list of Sockets
    /// </summary>
    /// <param name="identifier">Identifier of Socket</param>
    /// <param name="socket">Socket</param>
    /// <returns></returns>
    public static bool Add(string identifier, Socket socket)
    {
        return Clients.TryAdd(identifier, socket);
    }

    /// <summary>
    ///     Retrieve Socket by Identifier
    /// </summary>
    /// <param name="identifier">Identifier</param>
    /// <returns></returns>
    public static Socket? Get(string identifier)
    {
        Clients.TryGetValue(identifier, out var socket);
        return socket;
    }
}