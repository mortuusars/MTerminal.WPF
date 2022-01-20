using MTerminal.WPF.ViewModels;
using MTerminal.WPF.Windows;
using System.Windows.Media;

namespace MTerminal.WPF;

/// <summary>
/// Provides functionality to write text to a Terminal window and execute defined commands.
/// </summary>
public static class Terminal
{
    /// <summary>
    /// Output of the Terminal. Can be set as output of a <see cref="Console"/> by using <see cref="Console.SetOut(System.IO.TextWriter)"/>.
    /// </summary>
    public static TerminalWriter Out { get; }

    /// <summary>Provides access to Terminal Commands.</summary>
    public static Commands Commands { get; }

    /// <summary>Collection of properties that can be changed to alter Terminal window appearance.</summary>
    public static TerminalStyle Style { get; }

    /// <summary>Controls whether written text will be kept when window is closed and reopened again.</summary>
    //public static bool KeepWrittenDataBetweenWindows { get => _windowControl.KeepWrittenData; set => _windowControl.KeepWrittenData = value; }

    /// <summary>Version of the Terminal.</summary>
    public static readonly Version Version = new Version("0.2.0");


    static Terminal()
    {
        Commands = new Commands();
        _windowControl = new TerminalWindowControl();
        Style = new TerminalStyle(_windowControl.TerminalViewModel);
        Out = new TerminalWriter(_windowControl.TerminalWindow);
    }

    #region Window
    internal static TerminalWindow TerminalWindow { get => _windowControl.TerminalWindow; }

    private static readonly TerminalWindowControl _windowControl;

    /// <summary>
    /// Controls the maximum amount of lines of a text "buffer". When the limit is reached - oldest lines will be removed.<br>Default is 300.</br>
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If value is less or equal to 0.</exception>
    public static int BufferCapacity
    {
        get => _bufferCapacity;
        set
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(nameof(value));
            _bufferCapacity = value; 
            TerminalWindow.BufferCapacity = value;
        }
    }
    private static int _bufferCapacity = 300;

    /// <summary>
    /// Indicates whether Terminal window should save its text when closed and restore it when opened again.
    /// </summary>
    public static bool KeepWrittenTextWhenClosed { get; set; } = true;

    /// <summary>
    /// Opens Terminal window.
    /// </summary>
    public static void ShowWindow() => _windowControl.Show();
    /// <summary>
    /// Closes Terminal window if it is opened.
    /// </summary>
    public static void CloseWindow() => _windowControl.Close();
    /// <summary>
    /// Hides Terminal window if it is opened.
    /// </summary>
    public static void HideWindow() => _windowControl.Hide();

    #endregion

    #region Writing

    public static string Text { get => TerminalWindow.Text; }

    /// <summary>
    /// Writes a message to the Terminal Output.
    /// </summary>
    /// <param name="message">Message to write.</param>
    public static void Write(string message) => Out.Write(message);

    /// <summary>
    /// Writes a message of a specified color to the Terminal Output.
    /// </summary>
    /// <param name="message">Message to write.</param>
    public static void Write(string message, Color color) => Out.Write(message, color);

    /// <summary>
    /// Writes a new line to the output.
    /// </summary>
    public static void WriteLine() => Out.WriteLine();

    /// <summary>
    /// Writes a message followed by a new line to the output.
    /// </summary>
    /// <param name="message">Message to write.</param>
    public static void WriteLine(string message) => Out.WriteLine(message);

    /// <summary>
    /// Writes a colored message followed by a new line to the output.
    /// </summary>
    /// <param name="message">Message to write.</param>
    /// <summary>
    /// Writes a string representation of an object to the Terminal Output.
    /// </summary>
    /// <param name="value">Object to write.</param>
    public static void WriteLine(object value) => Out.WriteLine(value);

    /// <summary>
    /// Writes a colored string representation of an object to the Terminal Output.
    /// </summary>
    /// <param name="value">Object to write.</param>
    public static void WriteLine(object? value, Color color) => Out.WriteLine(value?.ToString() ?? "null", color);

    /// <summary>
    /// Clears Terminal screen.
    /// </summary>
    public static void Clear() => TerminalWindow.Clear();
    /// <summary>
    /// Clears last line of the output.
    /// </summary>
    public static void ClearLastLine() => TerminalWindow.RemoveLastLine();


    public static bool IsNewLine() => TerminalWindow.IsNewLine();

    #endregion
}