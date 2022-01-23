using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MTerminal.WPF.Controls;

public partial class ConsoleControl : UserControl
{
    public static readonly DependencyProperty CaretBrushProperty =
    DependencyProperty.Register(nameof(CaretBrush), typeof(Brush), typeof(ConsoleControl), new PropertyMetadata(Brushes.White));

    public Brush CaretBrush
    {
        get { return (Brush)GetValue(CaretBrushProperty); }
        set { SetValue(CaretBrushProperty, value); }
    }

    public static readonly DependencyProperty BufferCapacityProperty =
    DependencyProperty.Register(nameof(BufferCapacity), typeof(int), typeof(ConsoleControl), new PropertyMetadata(500));

    /// <summary>
    /// Number of lines after which oldest lines will be deleted.
    /// </summary>
    public int BufferCapacity
    {
        get { return (int)GetValue(BufferCapacityProperty); }
        set { SetValue(BufferCapacityProperty, value); }
    }

    public static readonly DependencyProperty AutoScrollOnWriteProperty =
    DependencyProperty.Register(nameof(AutoScrollOnWrite), typeof(bool), typeof(ConsoleControl), new PropertyMetadata(true));

    /// <summary>
    /// Indicates whether console will scroll to bottom when new text has been written.
    /// </summary>
    public bool AutoScrollOnWrite
    {
        get { return (bool)GetValue(AutoScrollOnWriteProperty); }
        set { SetValue(AutoScrollOnWriteProperty, value); }
    }

    public ConsoleControl()
    {
        InitializeComponent();
        Loaded += ConsoleControl_Loaded;
        console.TextChanged += Console_TextChanged;
    }

    private void Console_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Remove first line to prevent performance degradation due to many elements.
        if (console.Document.Blocks.Count > BufferCapacity)
            console.Document.Blocks.Remove(console.Document.Blocks.FirstBlock);
    }

    private void ConsoleControl_Loaded(object sender, RoutedEventArgs e)
    {
        ScrollViewer? scrollViewer = this.GetDescendantByType<ScrollViewer>();
        if (scrollViewer is not null)
        {
            scrollViewer.ScrollChanged += (s, e) =>
            {
                if (AutoScrollOnWrite) return;
                //TODO: enable auto-scroll when scrolled to bottom.
            };
        }
    }

    /// <summary>
    /// Tries to save current console content as xaml file.
    /// </summary>
    /// <param name="filePath">Path to the file.</param>
    /// <returns><see langword="false"/> if failed.</returns>
    public bool SaveXamlDocument(string filePath)
    {
        try
        {
            TextRange range = new TextRange(console.GetStartPointer(), console.GetEndPointer());
            using var fStream = new FileStream(filePath, FileMode.Create);
            range.Save(fStream, DataFormats.XamlPackage);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Tries to load contents of the console from a xaml file.
    /// </summary>
    /// <param name="filePath">Path to the file.</param>
    /// <returns><see langword="false"/> if failed.</returns>
    public bool LoadXamlDocument(string filePath)
    {
        if (!File.Exists(filePath))
            return false;

        try
        {
            TextRange range = new TextRange(console.Document.ContentStart, console.Document.ContentEnd);
            using var fStream = new FileStream(filePath, FileMode.Create);
            range.Load(fStream, DataFormats.XamlPackage);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Writes a string with a specified color to the console.<br>Executed on a UI thread.</br>
    /// </summary>
    /// <param name="value">Text to write.</param>
    /// <param name="color">Color of the text.</param>
    
    public void Write(string value, Color color) => RunOnUIDispatcher(() => Write(value, new SolidColorBrush(color)));
    /// <summary>
    /// Writes a string to the console.<br>Executed on a UI thread.</br>
    /// </summary>
    /// <param name="value">Text to write.</param>
    public void Write(string value) => RunOnUIDispatcher(() => Write(value, Foreground));
    
    private void Write(string value, Brush brush)
    {
        var range = new TextRange(console.GetEndPointer(), console.GetEndPointer())
        {
            Text = value
        };
        range.ApplyPropertyValue(TextElement.ForegroundProperty, brush);

        if (AutoScrollOnWrite)
            ScrollToEnd();
    }

    /// <summary>
    /// Inserts a string at the specified position.<br>Executed on a UI thread.</br>
    /// </summary>
    /// <param name="value">Text to insert.</param>
    /// <param name="position">Position of the insertion.</param>
    public void Insert(string value, int position)
    {
        RunOnUIDispatcher(() =>
        {
            console.CaretPosition = console.Document.ContentStart.GetPositionAtOffset(position, LogicalDirection.Forward);
            console.CaretPosition.InsertTextInRun(value);
        });
    }

    /// <summary>
    /// Gets all text that is currently displayed in a console.
    /// </summary>
    public string GetText()
    {
        return RunOnUIDispatcher(() => new TextRange(console.Document.ContentStart, console.GetEndPointer()).Text);
    }

    /// <summary>
    /// Removes last line from the console.<br>Executed on a UI thread.</br>
    /// </summary>
    public void RemoveLastLine()
    {
        RunOnUIDispatcher(() =>
        {
            var adj = console.Document.ContentEnd.GetAdjacentElement(LogicalDirection.Backward) as Block;
            console.Document.Blocks.Remove(adj);
        });
    }
    
    /// <summary>Scrolls console to the end, so the last written text is visible.<br>Executed on a UI thread.</br></summary>
    public void ScrollToEnd() => RunOnUIDispatcher(() => console.ScrollToVerticalOffset(double.MaxValue));
    
    /// <summary>Clears all text in a console.<br>Executed on a UI thread.</br></summary>
    public void Clear() => RunOnUIDispatcher(() => console.Document.Blocks.Clear());

    /// <summary>
    /// Executes Action on the UI Dispatcher.
    /// </summary>
    /// <param name="action">Action that will be executed.</param>
    private void RunOnUIDispatcher(Action action)
    {
        if (Dispatcher.CheckAccess())
            action();
        else
            Dispatcher.BeginInvoke(action);
    }

    /// <summary>
    /// Executes a fucntion on a UI Dispatcher.
    /// </summary>
    /// <typeparam name="T">Return type.</typeparam>
    /// <param name="func">Function to execute.</param>
    /// <returns>Result of the fucntion.</returns>
    private T RunOnUIDispatcher<T>(Func<T> func)
    {
        if (Dispatcher.CheckAccess())
            return func();
        else
            return Dispatcher.Invoke(func);
    }
}