using System.Net.Sockets;
using Common;
using Common.Models;

namespace BrokerServer;

public static class SocketHelper
{
            public static async Task StartAcceptingClientsAsync(this Socket server)
        {
            while (true)
            {
                try
                {
                    var handler = await server.AcceptAsync(); // Accept a new client connection
                    _ = Task.Run(() => HandleClientAsync(handler)); // Handle the client in a separate task
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while accepting client: {ex.Message}");
                }
            }
        }

        private static async Task HandleClientAsync(Socket handler)
        {
            try
            {
                while (true)
                {
                    var buffer = new byte[1_024];
                    var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                    if (received == 0)
                    {
                        Console.WriteLine("Client disconnected.");
                        break;
                    }

                    var response = new MessageResponse();
                    var message = received.FromBytes(buffer).JsonDeserialize<Message>();

                    if (await CheckIfMessageIsNullAsync(handler, message, response)) continue;
                    if (!await CheckIfMessageHaveSenderAsync(handler, message, response)) continue;
                    if (await AddClientIfNeededAsync(handler, message, response)) continue;
                    if (!await CheckIfMessageHaveRecipientsAsync(handler, message, response)) continue;

                    await ForwardMessageToRecipientsAsync(message, response);
                    await handler.SendAsync(response.JsonSerialize().ToBytes(), SocketFlags.None);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while handling client: {ex.Message}");
            }
        }

    private static async Task SendMessage<T>(this Socket socket, T message)
    {
        await socket.SendAsync(message.JsonSerialize().ToBytes(), SocketFlags.None);
    }
    private static async Task<bool> CheckIfMessageIsNullAsync(Socket handler, Message? message, MessageResponse response)
    {
        if (message is not null) return false;
        var deserializeErrorMessage =
            "After deserialization, message was empty. Message will not be processed".AddInfoPrefix();
        response.HasError = true;
        response.Messages.Add(deserializeErrorMessage);
        Console.WriteLine(deserializeErrorMessage);
        await handler.SendMessage(response);
        return true;
    }
    
    private static async Task<bool> CheckIfMessageHaveSenderAsync(Socket handler, Message? message, MessageResponse response)
    {
        if (!string.IsNullOrEmpty(message?.From)) return true;
        var error = "No Identifier provided. Message will not be processed".AddInfoPrefix();
        response.Messages.Add(error);
        Console.WriteLine(error);
        await handler.SendMessage(response);
        return false;
    }
    
    private static async Task<bool> CheckIfMessageHaveRecipientsAsync(Socket handler, Message? message, MessageResponse response)
    {
        if (message is not { To.Count: 0 }) return true;
        var errorMessage = "No receiver provided. Message will not be sent to any  clients".AddInfoPrefix();
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
            response.Messages.Add("Client was registered successfully".AddInfoPrefix());
        else
        {
            response.Messages.Add("An error occured when adding client, try another identifier".AddInfoPrefix());
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
                    $"No client with identifier {receiverIdentifier}. He will not get the message".AddInfoPrefix();
                response.Messages.Add(noClientMessage);
                Console.WriteLine(noClientMessage);
            }
            else
            {
                var msg = new MessageResponse();
                msg.Messages.Add("----------------------------------------------------------");
                msg.Messages.Add($"You got a message from {message.From}");
                msg.Messages.Add(message.JsonContent.ToString()!);
                msg.Messages.Add("----------------------------------------------------------");
                await client.SendMessage(msg);
                var successMessage = $"Message was sent to {receiverIdentifier}".AddInfoPrefix();
                response.Messages.Add(successMessage);
                Console.WriteLine(successMessage + $" from {message.From}");
            }
        }
    }
}