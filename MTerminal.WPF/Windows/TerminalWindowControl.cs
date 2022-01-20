using MTerminal.WPF.ViewModels;

namespace MTerminal.WPF.Windows;

/// <summary>
/// Controls showing, closing, hiding, etc. of the Terminal window.
/// </summary>
internal class TerminalWindowControl
{
    internal TerminalWindow TerminalWindow { get; private set; }
    internal TerminalViewModel TerminalViewModel { get; }

    internal TerminalWindowControl()
    {
        TerminalViewModel = new TerminalViewModel();
        TerminalWindow = CreateWindow();
    }

    private bool _isWindowClosed;

    internal void Show()
    {
        if (_isWindowClosed)
            TerminalWindow = CreateWindow();

        TerminalWindow.Show();
        _isWindowClosed = false;
    }

    internal void Close()
    {
        TerminalWindow.Close();
        _isWindowClosed = true;
    }

    internal void Hide() => TerminalWindow.Hide();

    private TerminalWindow CreateWindow()
    {
        if (TerminalWindow is not null)
            TerminalWindow.Closed -= Window_Closed;

        TerminalWindow window = new();

        window.DataContext = TerminalViewModel;

        window.Closed += Window_Closed;
        return window;
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        _isWindowClosed = true;
    }
}
