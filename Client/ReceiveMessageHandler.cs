using System.Net.Sockets;
using Common;
using Common.Models;

namespace Client;

public static class ReceiveMessageHandler
{
    public static async Task StartReceivingMessages()
    {
        var buffer = new byte[1_024];
        while (true)
        {
            try
            {
                var received = await ClientData.Socket.ReceiveAsync(buffer, SocketFlags.None);
                var message = received.FromBytes(buffer).JsonDeserialize<Message>();
                Console.WriteLine($"Got a message from : {message?.From}: \n {message?.JsonContent}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured when trying to receive message: {e.Message}");
            }
        }
    }
}