namespace Common.Models;

public class Message
{
    public string From { get; set; }
    public List<string> To { get; set; } = new();
    public object JsonContent { get; set; }
}