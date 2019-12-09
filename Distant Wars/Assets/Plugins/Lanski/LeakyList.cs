using System;

namespace Plugins.Lanski
{
    public class LeakyList<T>
        where T: struct
    {
        public LeakyList()
        {
            data = new T[4];
        }

        public int Count => count;

        public T this[int i] => data[i];
        
        public void Add(T item)
        {
            if (count == data.Length)
            {
                var tmp = data;
                data = new T[data.Length * 2];
                Array.Copy(tmp, data, tmp.Length);
            }

            data[count] = item;
            count++;
        }

        public void RemoveLast(int count)
        {
            var result = this.count - count;
            if (result < 0)
                throw new InvalidOperationException("count must be less than the size of the array");

            this.count = result;
        }

        private int count;
        private T[] data;

        internal void Clear()
        {
            count = 0;
        }
    }
}