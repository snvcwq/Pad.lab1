namespace Common.Models;

[Serializable]
public class MessageResponse
{
    public readonly List<string> Messages = [];
    public bool HasError = false;
}