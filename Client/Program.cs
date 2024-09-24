using System.Net.Sockets;   
using System.Text;
using Common;

var (client, ipEndPoint) = await SocketSettings.CreateSocket();
await client.ConnectAsync(ipEndPoint);

Console.WriteLine("Connected to the broker.");

// Start receiving messages in the background
_ = Task.Run(ReceiveMessagesAsync);

while (true)
{
    Console.WriteLine("Insert message:");
    var message = Console.ReadLine();
    
    if (string.IsNullOrEmpty(message))
    {
        Console.WriteLine("Empty message, not sent.");
        continue;
    }

    var messageBytes = Encoding.UTF8.GetBytes(message);
    await client.SendAsync(messageBytes, SocketFlags.None);
}

async Task ReceiveMessagesAsync()
{
    var buffer = new byte[1024];
    while (true)
    {
        var received = await client.ReceiveAsync(buffer, SocketFlags.None);
        var message = Encoding.UTF8.GetString(buffer, 0, received);
        Console.WriteLine($"Received: {message}");
    }
}