namespace Common;

public static class StringHelper
{
    public static string AddInfoPrefix(this string str)
    {
        return $"[INFO] {str}";
    }

    public static string AddInsertPrefix(this string str)
    {
        return $"[-->] {str}";
    }

    public static string AddBrokerPrefix(this string str)
    {
        return $"[FromBroker] {str}";
    }
}