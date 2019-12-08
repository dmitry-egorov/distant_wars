using UnityEngine;

public static class NullableExtensions
{
    public static bool _<T>(this T r) => true;

    public static bool try_get<T>(this T? n, out T v)
        where T: struct
    {
        if (n.HasValue)
        {
            v = n.Value;
            return true;
        }

        v = default;
        return false;
    }

    public static bool try_get<T>(this T n, out T v)
        where T: Object
    {
        if (n != null)
        {
            v = n;
            return true;
        }

        v = default;
        return false;
    }
}