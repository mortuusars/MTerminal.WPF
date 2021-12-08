using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Media;

namespace MTerminal.WPF;

public class TerminalWriter : TextWriter, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public int BufferCapacity { get; set; }

    public string WindowTitle
    {
        get => _windowTitle; set {
            _windowTitle = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTitle)));
        }
    }

    public SolidColorBrush DefaultForegroundColor
    {
        get => _defaultForegroundColor; set
        {
            _defaultForegroundColor = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DefaultForegroundColor)));
        }
    }

    public TerminalWindow? Window { get; private set; }

    public override Encoding Encoding => _encoding;
    private Encoding _encoding = Encoding.Default;

    private SolidColorBrush _defaultForegroundColor;
    private string _windowTitle;

    private bool _throwExceptions = true;

    internal TerminalWriter(bool throwExceptions)
    {
        _throwExceptions = throwExceptions;

        _defaultForegroundColor = Brushes.DarkGray;
        _windowTitle = "MTerminal";

        BufferCapacity = 2000;
    }

    public void SetEncoding(Encoding encoding) => _encoding = encoding;

    internal void ShowTerminalWindow()
    {
        CloseTerminalWindow();

        Window = new TerminalWindow() { DataContext = this };
        Window.Show();
    }

    internal void CloseTerminalWindow()
    {
        Window?.Close();
        Window = null;
    }

    internal void ClearScreen() => Window?.ClearScreen();

    internal void ClearLastLine() => Window?.ClearLastLine();

    // Overriding only these methods should be enough for terminal needs. Other overloads inplemented in base class.

    public override void WriteLine()
    {
        try
        {
            Window?.Write(new string(CoreNewLine));
        }
        catch (Exception)
        {
            if (_throwExceptions)
                throw;
        }
    }

    public override void Write(char value)
    {
        try
        {
            Window?.Write(value.ToString());
        }
        catch (Exception)
        {
            if (_throwExceptions)
                throw;
        }
    }

    public override void Write(char[] buffer, int index, int count)
    {
        if (buffer == null) throw new ArgumentNullException("buffer");
        if (index < 0) throw new ArgumentOutOfRangeException("index");
        if (count < 0) throw new ArgumentOutOfRangeException("count");
        if (buffer.Length - index < count) throw new ArgumentException("'Count' cannot be larger than a number of available chars.");

        var span = new Span<char>(buffer, index, count);

        try
        {
            Window?.Write(new string(span));
        }
        catch (Exception)
        {
            if (_throwExceptions)
                throw;
        }
    }
}