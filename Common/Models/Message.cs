namespace Common.Models;
[Serializable]
public class Message
{
    public bool RegisterClient = false;
    public string From { get; set; }
    public List<string> Topics { get; set; } = [];
    public object JsonContent { get; set; }
}