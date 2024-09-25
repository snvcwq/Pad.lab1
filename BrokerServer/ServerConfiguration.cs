using System.Net;
using System.Net.Sockets;

namespace BrokerServer;

public static class ServerConfiguration
{
    public static void StartListening(this Socket server, IPEndPoint ipEndPoint)
    {
        server.Bind(ipEndPoint);
        server.Listen();
        Console.WriteLine($"Server started Listening on port {ipEndPoint.Port}");
    }
}