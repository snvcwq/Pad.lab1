namespace Common.Models;
[Serializable]
public class Message
{
    public string From { get; set; }
    public List<string> To { get; set; } = [];
    public object JsonContent { get; set; }
}