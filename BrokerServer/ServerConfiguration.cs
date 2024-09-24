using System.Net;
using System.Net.Sockets;

namespace BrokerServer;

public static class ServerConfiguration
{
    public static async Task StartListening(this Socket server, IPEndPoint ipEndPoint)
    {
        await server.ConnectAsync(ipEndPoint);
        server.Bind(ipEndPoint);
        server.Listen();
    }
}