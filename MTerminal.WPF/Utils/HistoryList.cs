using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTerminal.WPF.Utils;

internal class HistoryList<T> : IEnumerable<T>
{
    public int Capacity { get; }

    private readonly List<T> _list;

    public HistoryList(int capacity)
    {
        Capacity = capacity;
        _list = new List<T>();
    }

    public void Add(T item)
    {
        if (_list.Count >= Capacity)
            _list.RemoveAt(0);

        _list.Add(item);
    }

    public T this[int i] { get => _list[i]; set => _list[i] = value; }
    public bool Remove(T item) => _list.Remove(item);

    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
}
