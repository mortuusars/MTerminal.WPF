using System.Windows.Media;

namespace MTerminal.WPF.ViewModels;

/// <summary>
/// Controls style parameters of the terminal window.
/// </summary>
public class TerminalStyle
{
    /// <summary>Title of the terminal window.</summary>
    public string WindowTitle { get => Terminal.TerminalViewModel.Title; set => Terminal.TerminalViewModel.Title = value; }

    /// <summary>Foreground color of the terminal window.</summary>
    public Color Foreground { get => Terminal.TerminalViewModel.Foreground.Color; set => Terminal.TerminalViewModel.Foreground = new SolidColorBrush(value); }

    /// <summary>Background color of the terminal window.</summary>
    public Brush Background { get => Terminal.TerminalViewModel.Background; set => Terminal.TerminalViewModel.Background = value; }

    /// <summary>Background color of the terminal window.</summary>
    public FontFamily FontFamily { get => Terminal.TerminalViewModel.FontFamily; set => Terminal.TerminalViewModel.FontFamily = value; }
}
