using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTerminal.WPF.Utils;

internal class CircularBuffer<T>
{
    public int Capacity { get; }

    private readonly T[] _buffer;
    private int _start;
    private int _end;

    public CircularBuffer(int capacity)
    {
        Capacity = capacity;

        _buffer = new T[capacity];
        _start = 0;
        _end = 0;
    }

    public T this[int i]
    {
        get => _buffer[(_start + i) % Capacity];
        set => _buffer[(_start + i) % Capacity] = value;
    }

    public void Add(T item)
    {
        _buffer[_end++] = item;
        _end %= Capacity;
    }

    public T GetNext()
    {
        T item = _buffer[_start++];
        _start %= Capacity;
        return item;
    }

    public T GetPrev()
    {
        T item = _buffer[_start--];
        if (_start < 0)
            _start += Capacity;
        return item;
    }
}
