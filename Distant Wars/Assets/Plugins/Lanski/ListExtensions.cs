using System.Collections.Generic;

namespace Plugins.Lanski
{
    public static class ListExtensions
    {
        public static bool TryGetFirstItem<T>(this List<T> l, out T item) => l.TryGetItemAt(0, out item);
        public static bool TryGetItemAt<T>(this List<T> l, int index, out T item)
        {
            if (index >= l.Count)
            {
                item = default;
                return false;
            }

            item = l[index];
            return true;
        }

        public static void ReserveMemoryFor<T>(this List<T> l, int count)
        {
            if (l.Capacity < count) l.Capacity = count;
        }
    }
}