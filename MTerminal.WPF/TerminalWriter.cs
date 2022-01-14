using System.IO;
using System.Text;
using System.Windows.Media;

namespace MTerminal.WPF;

public class TerminalWriter : TextWriter
{
    public override Encoding Encoding => _encoding;
    private Encoding _encoding = Encoding.Default;
    public void SetEncoding(Encoding encoding) => _encoding = encoding;

    internal void ClearScreen() => Terminal.Window?.ClearScreen();

    public void Write(string message, Color color) => Terminal.Window?.Write(message, color);
    public void WriteLine(string message, Color color)
    {
        Write(message, color);
        WriteLine();
    }
    internal void ClearLastLine() => Terminal.Window?.ClearLastLine();

    // Overriding only these methods should be enough for terminal needs. Other overloads inplemented in base class.

    public override void WriteLine() => Terminal.Window?.Write(new string(CoreNewLine));

    public override void Write(char value) => Terminal.Window?.Write(value.ToString());

    public override void Write(char[] buffer, int index, int count)
    {
        if (buffer == null) throw new ArgumentNullException("buffer");
        if (index < 0) throw new ArgumentOutOfRangeException("index");
        if (count < 0) throw new ArgumentOutOfRangeException("count");
        if (buffer.Length - index < count) throw new ArgumentException("'Count' cannot be larger than a number of available chars.");

        var span = new Span<char>(buffer, index, count);

        Terminal.Window?.Write(new string(span));
    }
}