namespace Plugins.Lanski
{
    public static class LeakyListUnityEx
    {
        public static bool TryGetFirstItem<T>(this RefLeakyList<T> l, out T item) where T: class => l.TryGetItemAt(0, out item);
        public static bool TryGetItemAt<T>(this RefLeakyList<T> l, int index, out T item)  where T: class
        {
            if (index >= l.Count)
            {
                item = default;
                return false;
            }

            item = l[index];
            return true;
        }

        public static int CleanUpExpiredObjects<T>(this RefLeakyList<T> list)
            where T: UnityEngine.Object
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