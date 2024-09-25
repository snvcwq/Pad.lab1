using System.Text;
using Newtonsoft.Json;

namespace Common;

public static class SerializationHelper
{
    public static string JsonSerialize(this object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    public static byte[] ToBytes(this string? str)
    {
        return Encoding.UTF8.GetBytes(str ?? string.Empty);
    }

    public static T? JsonDeserialize<T>(this string str)
    {
        T? value = default;
        try
        {
            return JsonConvert.DeserializeObject<T>(str);

        }
        catch (Exception)
        {
            return value;
        }
    }

    public static string FromBytes(this int received, byte[] buffer)
    {
        return Encoding.UTF8.GetString(buffer, 0, received);
    }
}