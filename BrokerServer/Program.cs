using BrokerServer;
using Common;

var socketSettings = new SocketSettings();
var (server, ipEndPoint) = await socketSettings.CreateSocket();
server.StartListening(ipEndPoint);
await server.StartAcceptingClientsAsync();