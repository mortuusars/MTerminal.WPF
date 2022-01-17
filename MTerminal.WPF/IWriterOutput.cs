using System.Windows.Media;

namespace MTerminal.WPF;

public interface IWriterOutput
{
    int BufferCapacity { get; set; }
    void Write(string text);
    void Write(string text, Color color);
    void ClearScreen();
    void ClearLastLine();
}