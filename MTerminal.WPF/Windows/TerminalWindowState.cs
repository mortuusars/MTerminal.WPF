using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace MTerminal.WPF.Windows;

internal record WindowStateRecord(double Width, double Height, double Left, double Top, double FontSize, WindowState WindowState);

internal static class TerminalWindowState
{
    /// <summary>
    /// Loads saved window state and sets window properties to those from loaded state.
    /// </summary>
    /// <param name="window">Window that properties will be set.</param>
    public static void Load(Window window)
    {
        if (GetStateFilePath(window.Title) is string stateFilePath && File.Exists(stateFilePath))
        {
            try
            {
                string json = File.ReadAllText(stateFilePath);
                WindowStateRecord? state = JsonSerializer.Deserialize<WindowStateRecord>(json);

                if (state is null)
                {
                    Terminal.WriteLine("Failed to deserialize TerminalWindowState: result was null.", Colors.DarkRed);
                    return;
                }

                window.Width = state.Width;
                window.Height = state.Height;
                window.Left = state.Left;
                window.Top = state.Top;
                window.FontSize = state.FontSize;
                window.WindowState = state.WindowState;
            }
            catch (Exception ex)
            {
                Terminal.WriteLine($"\nCannot restore terminal window settings: '{ex.Message}'", Colors.DarkRed);
            }
        }
    }

    /// <summary>
    /// Saves window state.
    /// </summary>
    /// <param name="window">Window which properties will be saved.</param>
    public static void Save(Window window)
    {
        if (GetStateFilePath(window.Title) is string stateFilePath)
        {
            try
            {
                WindowStateRecord state = new(window.Width,
                                              window.Height,
                                              window.Left,
                                              window.Top,
                                              window.FontSize,
                                              window.WindowState);

                string json = JsonSerializer.Serialize(state, new JsonSerializerOptions() { WriteIndented = true });

                File.WriteAllText(stateFilePath, json);
            }
            catch (Exception ex)
            {
                Terminal.WriteLine($"\nCannot save terminal window settings: '{ex.Message}'", Colors.DarkRed);
            }
        }
    }

    private static string? GetStateFilePath(string fileName)
    {
        try
        {
            string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string terminalFolder = Path.Combine(localFolder, "MTerminal");
            Directory.CreateDirectory(terminalFolder);
            string filePath = Path.Combine(terminalFolder, fileName + ".json");
            return filePath;
        }
        catch (Exception ex)
        {
            Terminal.WriteLine($"Cannot create window state filePath: '{ex.Message}'", Colors.DarkRed);
            return null;
        }
    }
}
