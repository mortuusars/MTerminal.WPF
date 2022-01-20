using System.Windows.Media;

namespace MTerminal.WPF;

/// <summary>
/// Destination of the writes.
/// </summary>
public interface IOutput
{
    void Write(string value);
    void Write(string value, Color color);
}