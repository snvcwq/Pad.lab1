using Grpc.Core;

namespace gRPCServer.Models;

public class Client
{
    public required string ClientId { get; set; }
    public List<string> Topics { get; set; } = new();
    public required IServerStreamWriter<TopicMessageResponse> Stream { get; set; }
}