using Client;
using Common;

var socketSettings = new SocketSettings();
var (client, ipEndPoint) = await socketSettings.CreateSocket();
await client.ConnectAsync(ipEndPoint);
RegistrationHandler.AddClientIdentifier();
await client.StarSendingMessagesAsync();