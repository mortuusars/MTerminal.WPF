using MTerminal.WPF.ViewModels;
using MTerminal.WPF.Windows;
using System.Windows.Media;

namespace MTerminal.WPF;

/// <summary>
/// Provides functionality to write text to a Terminal window and execute defined commands.
/// </summary>
public static class Terminal
{
    /// <summary>Version of the Terminal.</summary>
    public static readonly Version Version = new Version("0.2.0");
    /// <summary>Can be set as output of a <see cref="Console"/> by using <see cref="Console.SetOut(System.IO.TextWriter)"/>.</summary>
    public static TerminalWriter Out { get; }
    /// <summary>Provides access to Terminal Commands.</summary>
    public static Commands Commands { get; }
    /// <summary>Collection of properties that can be changed to alter Terminal window appearance.</summary>
    public static TerminalStyle Style { get; }

    internal static IOutput? Output { get => Window; }

    static Terminal()
    {
        Commands = new Commands();
        Style = new TerminalStyle();
        Out = new TerminalWriter();

        TerminalViewModel = new TerminalViewModel();
    }

    #region Window

    internal static TerminalWindow? Window { get; private set; }

    internal static TerminalViewModel TerminalViewModel { get; }

    /// <summary>
    /// Controls the maximum amount of lines of a text "buffer". When the limit is reached - oldest lines will be removed.<br>Default is 300.</br>
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If value is less or equal to 0.</exception>
    public static int BufferCapacity { get => TerminalViewModel.BufferCapacity; set => TerminalViewModel.BufferCapacity = value; }

    private static void CreateWindow()
    {
        if (Window is not null)
            Close();

        Window = new TerminalWindow();
        Window.DataContext = TerminalViewModel;
    }

    public static bool IsOpen { get => Window is not null; }
    public static bool IsVisible { get => Window is not null && Window.IsVisible; }

    public static void Show()
    {
        if (Window is null)
            CreateWindow();

        Window!.Show();
    }

    public static bool Close()
    {
        if (Window is null) return false;

        try
        {
            Window.Close();
            Window = null;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Hides Terminal window if it is opened.
    /// </summary>
    public static void Hide() => Window?.Hide();

    #endregion

    #region Writing

    public static string? Text { get => Window?.Text; }

    /// <summary>
    /// Writes a message to the Terminal Output.
    /// </summary>
    /// <param name="message">Message to write.</param>
    public static void Write(string message) => Window?.Write(message);

    /// <summary>
    /// Writes a new line to the output.
    /// </summary>
    public static void WriteLine() => Window?.WriteLine();

    /// <summary>
    /// Writes a message followed by a new line to the output.
    /// </summary>
    /// <param name="message">Message to write.</param>
    public static void WriteLine(string message) => Window?.WriteLine(message);

    /// <summary>
    /// Writes a colored message followed by a new line to the output.
    /// </summary>
    /// <summary>
    /// Writes a string representation of an object to the Terminal Output.
    /// </summary>
    /// <param name="value">Object to write.</param>
    public static void WriteLine(object value) => Window?.WriteLine(value?.ToString() ?? "null");

    /// <summary>
    /// Writes a colored string representation of an object to the Terminal Output.
    /// </summary>
    /// <param name="value">Object to write.</param>
    public static void WriteLine(object? value, Color color) => Window?.WriteLine(value?.ToString() ?? "null", color);


    /// <summary>
    /// Writes text of the specified color.
    /// </summary>
    /// <param name="value">Text to write.</param>
    /// <param name="color">Color of the text.</param>
    public static void Write(string value, Color color) => Window?.Write(value, color);

    /// <summary>
    /// Writes text of the specified color and moves caret to a new line.
    /// </summary>
    /// <param name="value">Text to write.</param>
    /// <param name="color">Color of the text.</param>
    public static void WriteLine(string value, Color color) => Window?.WriteLine(value, color);

    /// <summary>
    /// Clears Terminal screen.
    /// </summary>
    public static void Clear() => Window?.Clear();
    /// <summary>
    /// Clears last line of the output.
    /// </summary>
    public static void ClearLastLine() => Window?.RemoveLastLine();


    public static bool IsNewLine() => Window?.IsNewLine() ?? false;

    #endregion
}