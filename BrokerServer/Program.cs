using BrokerServer;
using Common;

var (server, ipEndPoint) = await SocketSettings.CreateSocket();
await server.StartListening(ipEndPoint);
await server.StartAcceptingClients();
