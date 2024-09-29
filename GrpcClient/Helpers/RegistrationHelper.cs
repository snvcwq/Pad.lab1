using CommongRPC;

namespace gRPCClient.Helpers;

public static class RegistrationHelper
{
    public static void AddClientIdentifier()
    {
        do
        {
            Console.WriteLine("Insert a valid Identifier:".AddInsertPrefix());
            ClientData.Identifier = Console.ReadLine()!;
        } while (string.IsNullOrEmpty(ClientData.Identifier));
    }
    
    public static TopicMessage AddClientTopics(this TopicMessage message)
    {
        int receivers;
        string receiversNumber;
        do
        {
            Console.WriteLine("Insert number of topics to which you want to subscribe:".AddInsertPrefix());
            receiversNumber = Console.ReadLine()!;
        } while (!int.TryParse(receiversNumber, out receivers));

        for (var i = 0; i < receivers; i++)
        {
            string receiver;
            do
            {
                Console.WriteLine("Insert topic name:".AddInsertPrefix());
                receiver = Console.ReadLine()!;
            } while (string.IsNullOrEmpty(receiver));

            message.Topics.Add(receiver);
        }
        return message;
    }
    
    public static TopicMessage SetRegistrationMethod(this TopicMessage message)
    {
        message.IsRegistrationMessage = true;
        return message;
    }
    
    public static TopicMessageResponse HandleRegistrationResponse(this TopicMessageResponse message)
    {
        Console.WriteLine($"Registration attempt ended with result: {message.IsSuccessful}".AddInfoPrefix());
        foreach (var msg in message.Messages)
        {
            Console.WriteLine(msg);
        }

        if (message.IsSuccessful)
            ClientData.IsRegistered = true;

        return message;
    }
    
}