using System.Net.Sockets;
using Common;
using Common.Models;

namespace Client;

public static class SendMessagesHandler
{
    public static async Task StartSendingMessages()
    {
        while (true)
        {
            try
            {
                var message = new Message();
                Console.WriteLine("Insert Message:");
                message.JsonContent = Console.ReadLine()!;
                if (string.IsNullOrEmpty(message.JsonContent.ToString()))
                {
                    Console.WriteLine("Was Inserted an empty string, message Insert a valid one");
                    continue;
                }

                int numberOfReceivers;
                string? insertedNumberOfReceivers;
                do
                {
                    insertedNumberOfReceivers = Console.ReadLine();
                } while (int.TryParse(insertedNumberOfReceivers, out numberOfReceivers) || numberOfReceivers <= 0);

                for (var i = 0; i < numberOfReceivers; i++)
                    message.To.Add(Console.ReadLine()!);
                message.From = ClientData.Identifier;

                await ClientData.Socket.SendAsync(message.JsonSerialize().ToBytes(), SocketFlags.None);
                var buffer = new byte[1_024];

                var received = await ClientData.Socket.ReceiveAsync(buffer, SocketFlags.None);
                var messageResponse = received.FromBytes(buffer).JsonDeserialize<MessageResponse>();
                Console.WriteLine(messageResponse is { HasError: true }
                    ? $"Server returned error: Messages: {string.Concat(",", messageResponse.Messages)}"
                    : $"Response from server {string.Concat(",", messageResponse?.Messages)}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured when trying to send message Message: {e.Message}");
            }
            
        }
    }
}