using System.Net.Sockets;

namespace Client;

public static class ClientData
{
    public static string Identifier { get; set; }

    public static Socket Socket { get; set; }
}