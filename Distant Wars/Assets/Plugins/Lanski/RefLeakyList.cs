using System;

namespace Plugins.Lanski
{
    // version of LeakyList for reference types; can cleanup its references
    public class RefLeakyList<T> where T: class
    {
        public RefLeakyList(int initial_capacity = 4)
        {
            data = new T[initial_capacity];
            leak_start = 0;
            leak_count = 0;
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
                leak_count = 0;
                leak_start = tmp.Length;
            }

            data[count] = item;
            count++;
            leak_count = leak_count > 0 ? leak_count - 1 : 0;
            leak_start++;
        }

        public void RemoveLast(int count)
        {
            var result = this.count - count;
            if (result < 0)
                throw new InvalidOperationException("count must be less than the size of the array");

            this.count = result;
            leak_count += count;
            leak_start -= count;
        }

        // write default values to the elements outside of the current list
        public void Cleanup()
        {
            Array.Clear(data, leak_start, leak_count);
            leak_count = 0;
            leak_start = count;
        }

        public void Clear()
        {
            leak_count += count;
            leak_start -= count;
            count = 0;
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        public struct Enumerator
        {
            public Enumerator(RefLeakyList<T> list)
            {
                this.i = -1;
                this.list = list;                
            }

            public bool MoveNext()
            {
                i++;
                return i < list.Count;
            }

            public T Current => list[i];

            int i;
            RefLeakyList<T> list;
        }

        private int count;
        private int leak_start;
        private int leak_count;
        private T[] data;
    }
}