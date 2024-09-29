using CommongRPC;

namespace gRPCClient.Helpers;

public static class MessageHelper
{
    public static TopicMessage SetMessageTopics(this TopicMessage message)
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

        return message;
    }
    
    public static TopicMessage SetMessageContent(this TopicMessage message)
    {
        string msg;
        do
        {
            Console.WriteLine("Insert Message:".AddInsertPrefix());
            msg = Console.ReadLine()!;
        } while (string.IsNullOrEmpty(msg));
        message.Message = msg;
        return message;
    }
    
    public static TopicMessage AddClientIdentifier(this TopicMessage message)
    {
        message.ClientId = ClientData.Identifier;
        return message;
    }
    
    public static TopicMessageResponse HandleMessageResponse(this TopicMessageResponse message)
    {
        Console.WriteLine($"Received a message from {message.ClientId}".AddInfoPrefix());
        foreach (var msg in message.Messages)
        {
            Console.WriteLine(msg);
        }

        return message;
    }
    
}