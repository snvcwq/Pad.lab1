using Client;
using Common;
using Common.Models;

var socketSettings = new SocketSettings();
var (client, ipEndPoint) = await socketSettings.CreateSocket();
await client.ConnectAsync(ipEndPoint);

RegistrationHandler.AddClientIdentifier();

while (true)
{
    try
    {
        var message = new Message { From = ClientData.Identifier };
        if (!ClientData.IsRegistered)
        {
            Console.WriteLine("Trying to register client");
            var result =await client.SendMessageAsync(message);
            if (!result.HasError)
                ClientData.IsRegistered = true;
        }
        else
        {
            Console.WriteLine("Sending Message:");
            SendMessagesHandler.SetMessageReceivers(message);
            SendMessagesHandler.SetMessageContent(message);
            await client.SendMessageAsync(message);
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"An error occured when trying to send message Message: {e.Message}");
    }
            
}

/*
await RegistrationHandler.RegisterClient();

_ = Task.Run(ReceiveMessageHandler.StartReceivingMessages);

_ = Task.Run(SendMessagesHandler.StartSendingMessages);*/