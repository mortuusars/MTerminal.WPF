using System.Windows.Media;

namespace MTerminal.WPF.ViewModels;

/// <summary>
/// Controls style parameters of the terminal window.
/// </summary>
public class TerminalStyle
{
    /// <summary>
    /// Title of the terminal window.
    /// </summary>
    public string WindowTitle
    {
        get => _terminalViewModel.Title;
        set => _terminalViewModel.Title = value;
    }

    /// <summary>
    /// Foreground color of the terminal window.
    /// </summary>
    public Color Foreground
    {
        get => _terminalViewModel.Foreground.Color;
        set => _terminalViewModel.Foreground = new SolidColorBrush(value);
    }

    /// <summary>
    /// Background color of the terminal window.
    /// </summary>
    public Brush Background
    {
        get => _terminalViewModel.Background;
        set => _terminalViewModel.Background = value;
    }

    /// <summary>
    /// Background color of the terminal window.
    /// </summary>
    public FontFamily FontFamily
    {
        get => _terminalViewModel.FontFamily;
        set => _terminalViewModel.FontFamily = value;
    }

    private readonly TerminalViewModel _terminalViewModel;
    
    internal TerminalStyle(TerminalViewModel terminalViewModel)
    {
        _terminalViewModel = terminalViewModel;
    }
}
