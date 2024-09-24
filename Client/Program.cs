using System.Net.Sockets;
using Common;
using Common.Models;

var socketSettings = new SocketSettings();
var (client, ipEndPoint) = await socketSettings.CreateSocket();
await client.ConnectAsync(ipEndPoint);



while (true)
{
    try
    {
        Console.WriteLine("Insert Message:");
       var message = new ClientRegistration("id");
        await client.SendAsync(message.JsonSerialize().ToBytes(), SocketFlags.None);
        var buffer = new byte[1_024];

        var received = await client.ReceiveAsync(buffer, SocketFlags.None);
        var messageResponse = received.FromBytes(buffer);
        Console.WriteLine(messageResponse);
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