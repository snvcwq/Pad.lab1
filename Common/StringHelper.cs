namespace Common;

public static class StringHelper
{
    public static string AddInfoPrefix(this string str)
    {
        return $"[INFO] {str}";
    }
}