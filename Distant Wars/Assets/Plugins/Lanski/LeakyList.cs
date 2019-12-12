using System;
using System.Collections;
using System.Collections.Generic;

namespace Plugins.Lanski
{
    public class LeakyList<T>: IList<T> where T: struct
    {
        public LeakyList(int initial_capacity = 4)
        {
            data = new T[initial_capacity];
        }

        public int Count => count;

        

        public ref T this[int i] => ref data[i];

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

        public void ReplaceWithLast(int i)
        {
            var last = count - 1;
            data[i] = data[last];
            RemoveLast(1);
        }

        public void Clear()
        {
            count = 0;
        }

        public Enumerator GetEnumerator() => new Enumerator(this);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool IsReadOnly => false;

        T IList<T>.this[int index] { get => this[index]; set => this[index] = value; }

        int IList<T>.IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        bool ICollection<T>.Contains(T item)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }

        public struct Enumerator: IEnumerator<T>
        {
            public Enumerator(LeakyList<T> list)
            {
                this.i = -1;
                this.list = list;                
            }

            public bool MoveNext()
            {
                i++;
                return i < list.Count;
            }

            public void Reset()
            {
                this.i = -1;
            }

            public void Dispose()
            {
            }

            public T Current => list[i];

            object IEnumerator.Current => Current;

            int i;
            LeakyList<T> list;
        }

        private int count;
        private T[] data;
    }
}