namespace CommongRPC;

public static class StringHelper
{
    public static string AddInfoPrefix(this string str) => $"[INFO] {str}";
    public static string AddInsertPrefix(this string str) => $"[-->] {str}";
    public static string AddBrokerPrefix(this string str) => $"[FromBroker] {str}";
}