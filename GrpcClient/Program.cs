using Grpc.Net.Client;
using gRPCClient;
using gRPCClient.Services;

using var channel = GrpcChannel.ForAddress("http://localhost:5001");
var client = new PubSub.PubSubClient(channel);
var call = client.SendMessageToTopic();
call.StartReceivingMessagesAsync();
await call.StartSendingMessagesAsync();