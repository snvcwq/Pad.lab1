using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Common.Models;

namespace BrokerServer;

public static class SocketHelper
{
    public static async Task StartAcceptingClients(this Socket server)
    {
        while (true)
        {
            var clientSocket = await server.AcceptAsync();
            
            var buffer = new byte[1024];
            var received = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
            var message = JsonSerializer.Deserialize<ClientRegistration>(received);
            if (message is null)
            {
                Console.WriteLine("Could not deserialize ClientRegistration message, client will not be added");
                continue;
            }
            SocketClients.Add(message.Identifier, clientSocket);
            
            _ = Task.Run(() => clientSocket.HandleClientMessagesAsync());
        }
    }

    private static async Task HandleClientMessagesAsync(this Socket clientSocket)
    {
        var buffer = new byte[1024];
    
        while (true)
        {
            var received = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
            var message = JsonSerializer.Deserialize<Message>(received);

            if (message is null)
            {
                Console.WriteLine("Could Not Deserialize Client's Message.Message will not be handled");
                continue;
            }


            var response = "Message received"u8.ToArray();
            await clientSocket.SendAsync(response, SocketFlags.None);

            // Forward message to other clients
            foreach (var client in clients)
            {
                if (client != clientSocket) // Do not send message back to sender
                {
                    await client.SendAsync(Encoding.UTF8.GetBytes(message), SocketFlags.None);
                }
            }
        }   
    }
}