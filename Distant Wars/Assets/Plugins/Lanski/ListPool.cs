using System;
using System.Collections.Generic;

public static class ListPool<T>
{
    public static Token Borrow(out List<T> list)
    {
        if (_unused.Count == 0)
        {
            _unused.Push(_lists.Count);
            _lists.Add(new List<T>());
        }

        var i = _unused.Pop();
        list = _lists[i];
        return new Token(i);
    }

    static void Return(int index)
    {
        _lists[index].Clear();
        _unused.Push(index);
    }

    static readonly List<List<T>> _lists = new List<List<T>>();
    static readonly Stack<int> _unused = new Stack<int>();
    
    public struct Token: IDisposable
    {
        public Token(int index)
        {
            _index = index;
        }

        public void Dispose()
        {
            Return(_index);
        }

        readonly int _index;
    }
}