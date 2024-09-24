using System.Net.Sockets;
using Common;
using Common.Models;

namespace BrokerServer;

public static class SocketHelper
{
    public static async Task HandleClientMessagesAsync(this Socket clientSocket)
    {
        await clientSocket.AcceptAsync();
        while (true)
            try
            {
                var buffer = new byte[1_024];
                var received = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
                var message = received.FromBytes(buffer).JsonDeserialize<Message>();
                var response = new MessageResponse();
                const string deserializeErrorMessage =
                    "Could Not Deserialize Client's Message. Message will not be handled";
                if (message is null)
                {
                    response.HasError = true;
                    response.Messages.Add(deserializeErrorMessage);
                    Console.WriteLine(deserializeErrorMessage);
                    await clientSocket.SendAsync(response.JsonSerialize().ToBytes(), SocketFlags.None);
                    continue;
                }

                response.Messages.Add("Message was received");
                var messageSender = SocketClients.Get(message.From);
                if (messageSender is null)
                    AddClient(message.From, clientSocket, response);

                foreach (var clientIdentifier in message.To)
                {
                    var client = SocketClients.Get(clientIdentifier);
                    if (client is null)
                    {
                        response.Messages.Add($"Server does not have client with Identifier {clientIdentifier}");
                    }
                    else
                    {
                        var msg = $"Message from {message.From}. \n Message: {message.JsonContent}";
                        await client.SendAsync(msg.JsonSerialize().ToBytes(), SocketFlags.None);
                        response.Messages.Add($"Message was sent to {clientIdentifier}.");
                    }
                }

                await clientSocket.SendAsync(response.JsonSerialize().ToBytes(), SocketFlags.None);
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured when processing a message. Message: {e.Message}");
                throw;
            }
    }

    private static void AddClient(string identifier, Socket client, MessageResponse response)
    {
        var isAdded = SocketClients.Add(identifier, client);
        response.Messages.Add(isAdded ? "Client was successfully added" : "An error occured when trying to add client");
    }
}