using System.Net.Sockets;
using BrokerServer.MessageModels;
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
            "After deserialization, message was empty. Message will not be processed".AddBrokerPrefix();
        response.HasError = true;
        response.Messages.Add(deserializeErrorMessage);
        Console.WriteLine(deserializeErrorMessage);
        await handler.SendMessage(response);
        return true;
    }
    
    private static async Task<bool> CheckIfMessageHaveSenderAsync(Socket handler, Message? message, MessageResponse response)
    {
        if (!string.IsNullOrEmpty(message?.From)) return true;
        var error = "No Identifier provided. Message will not be processed".AddBrokerPrefix();
        response.Messages.Add(error);
        Console.WriteLine(error);
        await handler.SendMessage(response);
        return false;
    }
    
    private static async Task<bool> CheckIfMessageHaveRecipientsAsync(Socket handler, Message? message, MessageResponse response)
    {
        if (message is not { Topics.Count: 0 }) return true;
        var errorMessage = "No topics provided. Message will not be sent to any  clients".AddBrokerPrefix();
        response.Messages.Add(errorMessage);
        Console.WriteLine(errorMessage + $"from {message.From}");
        await handler.SendMessage(response);
        return false;
    }
    private static async Task<bool> AddClientIfNeededAsync(Socket handler, Message? message, MessageResponse response)
    {
        if (!message.RegisterClient) 
            return false;
        foreach (var topic in message.Topics)
            Topics.Add(topic, (handler, message.From));
        response.Messages.Add($"Client successfully subscribed to topics".AddBrokerPrefix());
        await handler.SendMessage(response);
        var unconsumedMessages = ConsumedMessage.GetUnconsumedMessages(message.From, message.Topics);
        foreach (var msg in unconsumedMessages)
            await handler.SendMessage(msg);
        return true;
    }
    
    private static async Task ForwardMessageToRecipientsAsync(Message? message, MessageResponse response)
    {
        
        await Parallel.ForEachAsync(message?.Topics!,  async (topic, _) =>
        {
            var msg = new MessageResponse();
            msg.Messages.Add("----------------------------------------------------------");
            msg.Messages.Add($"You got a message from {message.From} using topic {topic}");
            msg.Messages.Add(message.JsonContent.ToString()!);
            msg.Messages.Add("----------------------------------------------------------");
            var msgGuid = UniqueMessage.Add(msg);
            var clients = Topics.GetTopicClients(topic);
            if(!clients.Any())
                response.Messages.Add($"No client is consuming {topic} topic".AddBrokerPrefix());
            else
            {
                await Parallel.ForEachAsync(clients, _, async (client, _) =>
                {
                        await client.Item1.SendMessage(msg);
                        ConsumedMessage.Add(client.Item2, msgGuid);
                        
                });
                response.Messages.Add($"All {topic} consumers got the message".AddBrokerPrefix());
            }
            Topics.AddMessage(topic, msgGuid);
        });
    }
}