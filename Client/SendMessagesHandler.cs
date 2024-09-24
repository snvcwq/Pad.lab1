using System.Net.Sockets;
using System.Text;

namespace Client;

public class SendMessagesHandler
{
    public static async Task HandleSendingMessages(Socket client)
    {
        while (true)
        {
            Console.WriteLine("Insert Message:");
            var message = Console.ReadLine();
            if (string.IsNullOrEmpty(message))
            {
                Console.WriteLine("Was Inserted an empty string, message will not be sent");
                continue;
            }
            var messageBytes = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(messageBytes, SocketFlags.None);
            var buffer = new byte[1_024];

            var received = await client.ReceiveAsync(buffer, SocketFlags.None);

            var receiverMessage = Encoding.UTF8.GetString(buffer, 0, received);
            Console.WriteLine(message);
        }
    }
}