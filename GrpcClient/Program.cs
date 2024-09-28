using Grpc.Core;
using Grpc.Net.Client;
//using gRPCClient;
using gRPCSClient;
using gRPCServer;
using Grpc.Core;
using Grpc.Net.Client;
using gRPCSClient;

using var channel = GrpcChannel.ForAddress("http://localhost:5001");
var client = new Greeter.GreeterClient(channel);

var call = client.SayHelloStream();

Task.Run(async () =>
{
    await foreach (var response in call.ResponseStream.ReadAllAsync())
    {
        Console.Write(response.Message);
    }

});

while (true)
{
    var result = Console.ReadLine();
    if (string.IsNullOrEmpty(result))
        break;
    await call.RequestStream.WriteAsync(new HelloRequest() { Name = result });
}
await call.RequestStream.CompleteAsync();
Console.ReadKey();
/*
using var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = new PubSubService.(channel);

// Create a new client object
var pubSubClient = new PubSubClient(client);

// Register client with an identifier and topics to subscribe to
string clientId = "Client1";
var topics = new List<string> { "topicA", "topicB" };
        
await pubSubClient.RegisterClientAsync(clientId, topics);
        
// Start sending messages to one of the topics
await pubSubClient.StartSendingMessagesAsync("topicA", clientId);
}*/