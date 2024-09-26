using System.Net.Sockets;
using Common;
using Common.Models;

namespace Client;

public static class SendMessagesHandler
{
    public static void SetMessageReceivers(Message message)
    {
        int receivers;
        string receiversNumber;
        do
        {
            Console.WriteLine("Insert number of topics to which you wanna send message".AddInsertPrefix());
            receiversNumber = Console.ReadLine()!;
        } while (!int.TryParse(receiversNumber, out receivers));

        for (var i = 0; i < receivers; i++)
        {
            string receiver;
            do
            {
                Console.WriteLine("Insert topic name".AddInsertPrefix());
                 receiver = Console.ReadLine()!;
            } while (string.IsNullOrEmpty(receiver));
            message.Topics.Add(receiver);
        }
    }

    public static void SetMessageContent(Message message)
    {
        string msg;
        do
        {
            Console.WriteLine("Insert Message:".AddInsertPrefix());
            msg = Console.ReadLine()!;
        } while (string.IsNullOrEmpty(msg));
        message.JsonContent = msg;
    }

    public static async Task<MessageResponse> SendMessageAsync(this Socket socket, Message message)
    {
        
        await socket.SendAsync(message.JsonSerialize().ToBytes(), SocketFlags.None);
        var buffer = new byte[1_024];

        var received = await socket.ReceiveAsync(buffer, SocketFlags.None);
        var messageResponse = received.FromBytes(buffer).JsonDeserialize<MessageResponse>();
        Console.WriteLine(messageResponse!.HasError
            ? "Sending Message ended with error".AddInfoPrefix()
            : "Sending Message ended successfully".AddInfoPrefix());

        foreach (var response in messageResponse.Messages)
            Console.WriteLine(response);
        return messageResponse;
    }
    
    public static void AddClientTopics(Message message)
    {
        int receivers;
        string receiversNumber;
        do
        {
            Console.WriteLine("Insert number of topics which messages, your client will consume".AddInsertPrefix());
            receiversNumber = Console.ReadLine()!;
        } while (!int.TryParse(receiversNumber, out receivers));

        for (var i = 0; i < receivers; i++)
        {
            string receiver;
            do
            {
                Console.WriteLine("Insert topic name".AddInsertPrefix());
                receiver = Console.ReadLine()!;
            } while (string.IsNullOrEmpty(receiver));
            message.Topics.Add(receiver);
        }
    }
}