public static class NullableExtensions
{
    public static bool TryGetValue<T>(this T? n, out T v)
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
}