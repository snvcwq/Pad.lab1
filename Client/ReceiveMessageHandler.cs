using System.Net.Sockets;
using Common;
using Common.Models;

namespace Client;

public static class ReceiveMessageHandler
{
    public static async Task StartReceivingMessagesAsync(Socket clientSocket)
    {
        try
        {
            while (true)
            {
                var buffer = new byte[1_024];
                var received = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
                if (received == 0)
                {
                    Console.WriteLine("Server disconnected.");
                    break;
                }

                var messageResponse = received.FromBytes(buffer).JsonDeserialize<MessageResponse>();
                if (messageResponse == null) continue;
                Console.WriteLine("Message received from server:".AddInfoPrefix());
                foreach (var response in messageResponse.Messages)
                {
                    Console.WriteLine(response);
                }
            }
        }
        catch (SocketException se)
        {
            Console.WriteLine($"SocketException occurred: {se.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while receiving messages: {ex.Message}");
        }
    }
}