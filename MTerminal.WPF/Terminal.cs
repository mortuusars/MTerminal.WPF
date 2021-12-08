using MTerminal.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MTerminal.WPF;

public static class Terminal
{
    public static TerminalWriter Out { get; }

    /// <summary>
    /// Text that will be displayed on a Terminal window header.
    /// </summary>
    public static string WindowTitle { get => Out.WindowTitle; set => Out.WindowTitle = value; }

    /// <summary>
    /// Controls the capacity of a text "buffer". Every write is 1 element. When the limit is reached - oldest elements would be removed.<br/>
    /// </summary>
    public static int BufferCapacity { get => Out.BufferCapacity; set => Out.BufferCapacity = value; }

    /// <summary>
    /// Default color of the output text.
    /// </summary>
    public static Color ForegroundColor { get => Out.DefaultForegroundColor.Color; set => Out.DefaultForegroundColor = new SolidColorBrush(value); }

    static Terminal()
    {
        Out = new TerminalWriter(throwExceptions: true);
    }

    /// <summary>
    /// Opens Terminal window. Closes already existing Terminal window.
    /// </summary>
    public static void ShowWindow()
    {
        Out.ShowTerminalWindow();
    }

    /// <summary>
    /// Closes Terminal window if opened.
    /// </summary>
    public static void CloseWindow() => Out.CloseTerminalWindow();

    public static void Write(string message) => Out.Write(message);
    public static void Write(string message, Color color) => Out.Write(message, color);
    public static void WriteLine(string message) => Out.WriteLine(message);
    public static void WriteLine(string message, Color color) => Out.WriteLine(message, color);
    public static void RemoveLastLine() => Out.ClearLastLine();

    /// <summary>
    /// Clears Terminal screen.
    /// </summary>
    public static void Clear() => Out.ClearScreen();
}