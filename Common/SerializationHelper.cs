using System.Text;
using System.Text.Json;

namespace Common;

public static class SerializationHelper
{
    public static string JsonSerialize(this object obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    public static byte[] ToBytes(this string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }

    public static T? JsonDeserialize<T>(this string str)
    {
        return JsonSerializer.Deserialize<T>(str);
    }

    public static string FromBytes(this int received, byte[] buffer)
    {
        return Encoding.UTF8.GetString(buffer, 0, received);
    }
}