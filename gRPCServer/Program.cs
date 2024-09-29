using gRPCServer.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
var app = builder.Build();
app.MapGrpcService<PubSubSvc>();
Console.WriteLine("Broker started to work");
app.Run();
