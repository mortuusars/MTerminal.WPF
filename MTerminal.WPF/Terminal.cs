using MTerminal.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace MTerminal.WPF;

public static class Terminal
{
    public static TerminalWriter Out { get; }

    public static Commands Commands { get; }

    /// <summary>
    /// Text that will be displayed on a Terminal window header.
    /// </summary>
    public static string WindowTitle { get => TerminalViewModel.WindowTitle; set => TerminalViewModel.WindowTitle = value; }

    /// <summary>
    /// Controls the capacity of a text "buffer". Every write is 1 element. When the limit is reached - oldest elements would be removed.<br/>
    /// </summary>
    public static int BufferCapacity { get => _bufferCapacity; set { _bufferCapacity = value; if (Window is not null) Window.BufferCapacity = value; } }
    private static int _bufferCapacity;

    /// <summary>
    /// Default color of the output text.
    /// </summary>
    public static Color ForegroundColor { get => TerminalViewModel.DefaultForeground.Color; set => TerminalViewModel.DefaultForeground = new SolidColorBrush(value); }

    public static readonly Version Version = new Version("0.1.0");

    internal static TerminalWindow? Window { get; private set; }
    internal static TerminalViewModel TerminalViewModel { get; private set; }

    static Terminal()
    {
        Out = new TerminalWriter();

        Commands = new Commands();

        CommandViewModel commandViewModel = new CommandViewModel(Commands);
        TerminalViewModel = new TerminalViewModel(commandViewModel);
    }

    /// <summary>
    /// Opens Terminal window. Closes already existing Terminal window.
    /// </summary>
    public static void ShowWindow()
    {
        if (Window is not null)
        {
            Window.WindowState = System.Windows.WindowState.Normal;
            Window.ShowInTaskbar = true;
        }
        else
        {
            Window = new TerminalWindow() { DataContext = TerminalViewModel, BufferCapacity = BufferCapacity };
            Window.Show();

        }
        //CloseWindow();

    }

    /// <summary>
    /// Closes Terminal window if opened.
    /// </summary>
    public static void CloseWindow()
    {
        //TODO: Proper window hiding/closing.
        if (Window is not null)
        {
            Window.WindowState = System.Windows.WindowState.Minimized;
            Window.ShowInTaskbar = false;
        }


        //Window?.Close();
        //Window = null;
    }

    public static void Write(string message) => Out.Write(message);
    public static void Write(string message, Color color) => Out.Write(message, color);
    public static void WriteLine() => Out.WriteLine();
    public static void WriteLine(object toPrint) => Out.WriteLine(toPrint);
    public static void WriteLine(object? toPrint, Color color) => Out.WriteLine(toPrint?.ToString() ?? "null", color);
    public static void WriteLine(string message) => Out.WriteLine(message);
    public static void WriteLine(string message, Color color) => Out.WriteLine(message, color);
    public static void RemoveLastLine() => Out.ClearLastLine();

    /// <summary>
    /// Clears Terminal screen.
    /// </summary>
    public static void Clear() => Out.ClearScreen();
}