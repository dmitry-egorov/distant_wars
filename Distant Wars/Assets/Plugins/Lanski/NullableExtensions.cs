public static class NullableExtensions
{
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
        where T: class
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