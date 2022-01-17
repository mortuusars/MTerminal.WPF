using MTerminal.WPF.ViewModels;
using System.Windows.Documents;

namespace MTerminal.WPF.Windows;

/// <summary>
/// Controls showing, closing, hiding, etc. of the Terminal window.
/// </summary>
internal class TerminalWindowControl
{
    public TerminalWindow TerminalWindow { get; private set; }
    internal TerminalViewModel TerminalViewModel { get; }

    public int BufferCapacity { get => _bufferCapacity; set { _bufferCapacity = value; TerminalWindow.BufferCapacity = value; } }
    public bool KeepWrittenData { get; set; }

    /// <summary>
    /// Stored text of the terminal. Used to restore it if window closes and reopens.
    /// </summary>
    private IEnumerable<Inline>? _writtenData;

    public TerminalWindowControl()
    {
        TerminalViewModel = new TerminalViewModel();
        TerminalWindow = CreateWindow();
    }

    private bool _isWindowClosed;
    private int _bufferCapacity = 1500;

    public void Show()
    {
        if (_isWindowClosed)
            TerminalWindow = CreateWindow();

        TerminalWindow.Show();
        _isWindowClosed = false;
    }

    public void Close()
    {
        try
        {
            _writtenData = TerminalWindow.GetInlines();
            TerminalWindow.Close();
            _isWindowClosed = true;
        }
        catch (Exception) { Terminal.WriteLine("Failed to close Terminal window."); }
    }

    public void Hide()
    {
        try { TerminalWindow.Hide(); }
        catch (Exception) { Terminal.WriteLine("Failed to hide Terminal window."); }
    }

    private TerminalWindow CreateWindow()
    {
        TerminalWindow window = new();

        if (KeepWrittenData && _writtenData is not null && _writtenData.Any())
            window.SetInlines(_writtenData);

        window.BufferCapacity = BufferCapacity;
        window.DataContext = TerminalViewModel;

        window.Closed += Window_Closed;
        return window;
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        _isWindowClosed = true;

        try { _writtenData = TerminalWindow.GetInlines(); }
        catch (Exception) { Terminal.WriteLine(); }
    }
}
