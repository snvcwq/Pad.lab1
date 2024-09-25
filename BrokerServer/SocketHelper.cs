using System.Net.Sockets;
using Common;
using Common.Models;

namespace BrokerServer;

public static class SocketHelper
{
    public static async Task StartAcceptingClientsAsync(this Socket server)
    {
        var handler = await server.AcceptAsync();
        while (true)
            try
            {
                var buffer = new byte[1_024];
                var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var response = new MessageResponse();
                var message = received.FromBytes(buffer).JsonDeserialize<Message>();
                if (await CheckIfMessageIsNullAsync(handler, message, response))
                    continue;
                if (!await CheckIfMessageHaveSenderAsync(handler, message, response))
                    continue;
                if (await AddClientIfNeededAsync(handler, message, response))
                    continue;
                if (!await CheckIfMessageHaveRecipientsAsync(handler, message, response))
                    continue;
                await ForwardMessageToRecipientsAsync(message, response);
                
                await handler.SendAsync(response.JsonSerialize().ToBytes(), SocketFlags.None);
            }
            catch (Exception)
            {
                Console.WriteLine("Error occured when accepting Messages");
            }
    }

    private static async Task SendMessage<T>(this Socket socket, T message)
    {
        await socket.SendAsync(message.JsonSerialize().ToBytes(), SocketFlags.None);
    }
    private static async Task<bool> CheckIfMessageIsNullAsync(Socket handler, Message? message, MessageResponse response)
    {
        if (message is not null) return false;
        const string deserializeErrorMessage =
            "After deserialization, message was empty. Message will not be processed";
        response.HasError = true;
        response.Messages.Add(deserializeErrorMessage);
        Console.WriteLine(deserializeErrorMessage);
        await handler.SendMessage(response);
        return true;
    }
    
    private static async Task<bool> CheckIfMessageHaveSenderAsync(Socket handler, Message? message, MessageResponse response)
    {
        if (!string.IsNullOrEmpty(message?.From)) return true;
        const string error = "No Identifier provided. Message will not be processed";
        response.Messages.Add(error);
        Console.WriteLine(error);
        await handler.SendMessage(response);
        return false;
    }
    
    private static async Task<bool> CheckIfMessageHaveRecipientsAsync(Socket handler, Message? message, MessageResponse response)
    {
        if (message is not { To.Count: 0 }) return true;
        const string errorMessage = "No receiver provided. Message will not be sent to any  clients";
        response.Messages.Add(errorMessage);
        Console.WriteLine(errorMessage + $"from {message.From}");
        await handler.SendMessage(response);
        return false;
    }
    private static async Task<bool> AddClientIfNeededAsync(Socket handler, Message? message, MessageResponse response)
    {
        if (SocketClients.Get(message!.From) is not null) return false;
        var isAdded = SocketClients.Add(message.From, handler);
        if (isAdded)
            response.Messages.Add("Client was registered successfully");
        else
        {
            response.Messages.Add("An error occured when adding client, try another identifier");
            response.HasError = true;
        }

        await handler.SendMessage(response);
        return true;
    }
    
    private static async Task ForwardMessageToRecipientsAsync(Message? message, MessageResponse response)
    {
        foreach (var receiverIdentifier in message!.To)
        {
            var client = SocketClients.Get(receiverIdentifier);
            if (client is null)
            {
                var noClientMessage =
                    $"No client with identifier {receiverIdentifier}. He will not get the message";
                response.Messages.Add(noClientMessage);
                Console.WriteLine(noClientMessage);
            }
            else
            {
                await client.SendMessage(response);
                var successMessage = $"Message was sent to {receiverIdentifier}";
                response.Messages.Add(successMessage);
                Console.WriteLine(successMessage + $"from {message.From}");
            }
        }
    }
}