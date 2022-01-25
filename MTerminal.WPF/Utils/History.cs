namespace MTerminal.WPF.Utils;

/// <summary>
/// Represents a list with a limited capacity. When items are added to the history - oldest items will be deleted.<br></br>
/// Allows traversing the list backwards and forwards, keeping the position.
/// Similar to the Circular Buffer, but more fitting to my needs.
/// </summary>
/// <typeparam name="T">Type of items that will be stored.</typeparam>
internal class History<T>
{
    /// <summary>
    /// Indicates that getting next or previous item should be cyclic: if called GetNext on last item - jump to the start. And vise versa.<br></br>
    /// Default is <see langword="true"/>.
    /// </summary>
    public bool CycleGettters { get; set; }

    /// <summary>
    /// Gets the maximum amount of items this history list can hold.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Gets the count of items in a history list.
    /// </summary>
    public int Count { get => _items.Count; }

    /// <summary>
    /// Starting traversing position.<br></br>
    /// -1 means previous item will be the last item and Next item will be the first item.<br></br>
    /// All other values will be decremented if requested previous items, incremented if requested next item.
    /// </summary>
    public int TraversingIndex { get; set; }

    /// <summary>
    /// Sets TraversingIndex to proper values.
    /// </summary>
    private int ReturnIndex
    {
        get => TraversingIndex;
        set
        {
            if (CycleGettters)
            {
                if (value >= Count)
                    TraversingIndex = 0;
                else if (value < 0)
                    TraversingIndex = Count - 1;
                else
                    TraversingIndex = value;

                return;
            }

            TraversingIndex = Math.Clamp(value, 0, Count - 1);
        }
    }

    private readonly List<T> _items;

    /// <summary>
    /// Creates new instance of a History with a specified capacity.
    /// </summary>
    /// <param name="capacity"></param>
    public History(int capacity)
    {
        CycleGettters = true;
        Capacity = capacity;
        _items = new List<T>();
    }

    /// <summary>
    /// Adds item to the end of a list.<br></br>
    /// Oldest item is removed if capacity is reached.<br></br>
    /// Resets TraversingIndex to -1.
    /// </summary>
    /// <param name="item"></param>
    public void Append(T item)
    {
        if (_items.Count >= Capacity)
            _items.RemoveAt(0);

        _items.Add(item);
        TraversingIndex = -1;
    }

    /// <summary>
    /// Returns items from a list going backwards. <see cref="CycleGettters"/> controls if it will jump to the last item if end is reached.
    /// </summary>
    /// <returns>Previous item in a history.</returns>
    public T GetPrevious()
    {
        --ReturnIndex;
        return _items[ReturnIndex];
    }

    /// <summary>
    /// Returns items from a list going backwards. <see cref="CycleGettters"/> controls if it will jump to the first item if end is reached.
    /// </summary>
    /// <returns>Next item in a history.</returns>
    public T GetNext()
    {
        ++ReturnIndex;
        return _items[ReturnIndex];
    }

    /// <summary>
    /// Get or set the item at a specified index.
    /// </summary>
    /// <param name="i">Index.</param>
    /// <exception cref="IndexOutOfRangeException">If index is not witthin a list bounds.</exception>
    public T this[int i]
    {
        get => _items[i];
        set => _items[i] = value;
    }
}
