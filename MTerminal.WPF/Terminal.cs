using MTerminal.WPF.Commands;
using MTerminal.WPF.ViewModels;
using MTerminal.WPF.Windows;
using System.Windows.Input;
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
    public static CommandRegistry Commands { get; }
    /// <summary>Collection of properties that can be changed to alter Terminal window appearance.</summary>
    public static TerminalStyle Style { get; }

    static Terminal()
    {
        Commands = new CommandRegistry();
        Style = new TerminalStyle();
        Out = new TerminalWriter();

        Commands.Add(new HelpTerminalCommand());
        Commands.Add(new TerminalCommand("clear", "Clears the Terminal screen.", (_) => Terminal.Clear()).AddAlias("cls"));

        TerminalViewModel = new TerminalViewModel();
    }

    /// <summary>
    /// Waits until a user inputs a character.<br></br>
    /// Use carefully: if GetAwaiter().GetResult() is used - it will result in a deadlock.
    /// </summary>
    /// <returns>Input character.</returns>
    /// <exception cref="InvalidOperationException">If Terminal window is closed.</exception>
    public static Task<char> Read()
    {
        if (Window is null) throw new InvalidOperationException("Terminal window is null.");
        return Window.Read();
    }

    /// <summary>
    /// Waits until a user presses return key and returns the text that was entered.<br></br>
    /// Use carefully: if GetAwaiter().GetResult() is used - it will result in a deadlock.
    /// </summary>
    /// <returns>Inputed line.</returns>
    /// <exception cref="InvalidOperationException">If Terminal window is closed.</exception>
    public static Task<string> ReadLine()
    {
        if (Window is null) throw new InvalidOperationException("Terminal window is null.");
        return Window.ReadLine();
    }

    /// <summary>
    /// Waits until a user presses a key. Modifier keys are ignored as a key, but included as part of a key combination.<br></br>
    /// Use carefully: if GetAwaiter().GetResult() is used - it will result in a deadlock.
    /// </summary>
    /// <returns>Pressed key with modifiers..</returns>
    /// <exception cref="InvalidOperationException">If Terminal window is closed.</exception>
    public static Task<(Key key, ModifierKeys modifiers)> ReadKey()
    {
        if (Window is null) throw new InvalidOperationException("Terminal window is null.");
        return Window.ReadKey();
    }

    #region Window

    internal static TerminalWindow? Window { get; private set; }

    internal static TerminalViewModel TerminalViewModel { get; }

    /// <summary>
    /// Controls the maximum amount of lines of a text "buffer". When the limit is reached - oldest lines will be removed.<br>Default is 300.</br>
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If value is less or equal to 0.</exception>
    public static int BufferCapacity { get => TerminalViewModel.BufferCapacity; set => TerminalViewModel.BufferCapacity = value; }

    /// <summary>
    /// Indicates whether the window was shown. (Window can be hidden but still open)
    /// </summary>
    public static bool IsOpen { get => Window is not null; }
    /// <summary>
    /// Indicates whether the Terminal window is visible on the screen.
    /// </summary>
    public static bool IsVisible { get => Window is not null && Window.IsVisible; }

    /// <summary>
    /// Show the terminal window.
    /// </summary>
    public static void Show()
    {
        (Window ??= CreateWindow()).Show();
    }

    /// <summary>
    /// Closes the terminal window.
    /// </summary>
    /// <returns><see langword="true"/> if window was successfully closed. Otherwise <see langword="false"/>.</returns>
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

    private static TerminalWindow CreateWindow()
    {
        if (Window is not null)
            Close();

        var window = new TerminalWindow();
        window.DataContext = TerminalViewModel;
        window.Closed += (s, e) => Close();
        return window;
    }

    #endregion

    #region Writing

    /// <summary>
    /// Gets the text that is currently on the Terminal screen.
    /// </summary>
    public static string? Text { get => Window?.Text; }

    /// <summary>
    /// Writes a message to the Terminal Output.
    /// </summary>
    /// <param name="message">Message to write.</param>
    public static void Write(string message)
    {
        Window?.Write(message);
    }

    /// <summary>
    /// Writes text of the specified color.
    /// </summary>
    /// <param name="value">Text to write.</param>
    /// <param name="color">Color of the text.</param>
    public static void Write(string value, Color color)
    {
        Window?.Write(value, color);
    }

    /// <summary>
    /// Writes a new line to the output.
    /// </summary>
    public static void WriteLine()
    {
        Window?.Write(Environment.NewLine);
    }

    /// <summary>
    /// Writes a message followed by a new line to the output.
    /// </summary>
    /// <param name="value">Message to write.</param>
    public static void WriteLine(string value)
    {
        Write(value);
        WriteLine();
    }

    /// <summary>
    /// Writes text of the specified color and moves caret to a new line.
    /// </summary>
    /// <param name="value">Text to write.</param>
    /// <param name="color">Color of the text.</param>
    public static void WriteLine(string value, Color color)
    {
        Write(value, color);
        WriteLine();
    }

    /// <summary>
    /// Writes a colored message followed by a new line to the output.
    /// </summary>
    /// <summary>
    /// Writes a string representation of an object to the Terminal Output.
    /// </summary>
    /// <param name="value">Object to write.</param>
    public static void WriteLine(object value)
    {
        WriteLine(value?.ToString() ?? "null");
    }

    /// <summary>
    /// Writes a colored string representation of an object to the Terminal Output.
    /// </summary>
    /// <param name="value">Object to write.</param>
    /// <param name="color">Color of the text.</param>
    public static void WriteLine(object? value, Color color)
    {
        WriteLine(value?.ToString() ?? "null", color);
    }

    /// <summary>Clears Terminal screen.</summary>
    public static void Clear()
    {
        Window?.Clear();
    }

    /// <summary>Clears last line of the output.</summary>
    public static void ClearLastLine()
    {
        Window?.RemoveLastLine();
    }

    /// <summary>Indicates that next write will be on a new line.</summary>
    public static bool IsNewLine()
    {
        return Text is not null && (Text.Length == 0 || Text.EndsWith(Environment.NewLine));
    }

    #endregion
}