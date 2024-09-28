using Grpc.Net.Client;
using gRPCClient;

RegistrationHandler.AddClientIdentifier();

using var channel = GrpcChannel.ForAddress("http://localhost:5001");
var client = new PubSub.PubSubClient(channel);

var pubSubClientService = new PubSubService(client);

await pubSubClientService.RegisterClientAsync();

await pubSubClientService.StartSendingMessagesAsync();