using BrokerServer;
using Common;

var socketSettings = new SocketSettings();
var (server, ipEndPoint) = await socketSettings.CreateSocket();
server.StartListening(ipEndPoint);
Console.WriteLine("Server Started Listening");
await server.HandleClientMessagesAsync();
