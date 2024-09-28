using Common;

namespace gRPCClient;

public static class SendMessagesHandler
{
    public static ClientRegistration SetMessageTopics(this ClientRegistration clientRegistration)
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
            clientRegistration.Topics.Add(receiver);
        }
        return clientRegistration;
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
    
    public static TopicMessage AddClientTopics(this TopicMessage message)
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

        return message;
    }
}