using System.Net.Sockets;
using Common;
using Common.Models;

namespace Client;

public static class RegistrationHandler
{
    public static async Task RegisterClient()
    {
        var isSuccessful = false;
        while (!isSuccessful)
        {
            try
            {
                string port;
                int portValue;
                do
                {
                    Console.WriteLine("Insert a valid port");
                    port = Console.ReadLine()!;
                } while (!int.TryParse(port, out portValue));
                do
                {
                    Console.WriteLine("Insert a valid Identifier");
                    ClientData.Identifier = Console.ReadLine()!;
                } while (string.IsNullOrEmpty(ClientData.Identifier));
                var socketSettings = new SocketSettings();
                var (client, ipEndPoint) = await socketSettings.CreateSocket();
                var registration = new ClientRegistration(ClientData.Identifier);
                await client.ConnectAsync(ipEndPoint);
                await client.SendAsync(registration.JsonSerialize().ToBytes(), SocketFlags.None);
                ClientData.Socket = client;
                isSuccessful = true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured when trying to register client {e.Message}");
                throw;
            }
        }
    }
}