using System.Net;
using System.Net.Sockets;

namespace Common;

public class SocketSettings
{
    private const int Port = 8001;

    /// <summary>
    /// Creates Socket with generic settings
    /// </summary>
    /// <returns>Created Socket and IPEndPoint of socket</returns>
    public async Task<(Socket, IPEndPoint)> CreateSocket()
    {
        var ipEndPoint = await CreateDefaultIpEndpoint();
        return (new Socket(
            ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp
        ), ipEndPoint);
    }

    private async Task<IPEndPoint> CreateDefaultIpEndpoint()
    {
        var ipEntry = await Dns.GetHostEntryAsync(Dns.GetHostName());
        var ip = ipEntry.AddressList.FirstOrDefault();
        if (ip is null)
            throw new Exception("IP Address is null");

        return new IPEndPoint(ip, Port);
    }
}