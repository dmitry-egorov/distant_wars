using System.Collections.Generic;
using UnityEngine;

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

        public static void ReplaceWithLast<T>(this List<T> l, int i)
        {
            var last = l.Count - 1;
            l[i] = l[last];
            l.RemoveAt(l.Count - 1);
        }

        public static int AddAndGetIndex<T>(this List<T> list, T e)
        {
            list.Add(e);
            return list.Count - 1;
        }

        public static int CleanUpExpiredObjects<T>(this List<T> list)
            where T: Object
        {
            var count = 0;
            var i = 0;
            while(i < list.Count)
            {
                if (list[i] == null)
                {
                    list.ReplaceWithLast(i);
                    count++;
                }
                else
                {
                    i++;
                }
            }

            return count;
        }
    }
}