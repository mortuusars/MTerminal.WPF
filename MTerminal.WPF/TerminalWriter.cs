using System.IO;
using System.Text;

namespace MTerminal.WPF;

/// <summary>
/// Used to replace output of a <see cref="Console"/>.
/// </summary>
public class TerminalWriter : TextWriter
{
    public override Encoding Encoding => _encoding;
    private Encoding _encoding = Encoding.Default;
    public void SetEncoding(Encoding encoding) => _encoding = encoding;

    public override void WriteLine() => Terminal.Write(new string(CoreNewLine));

    public override void Write(char value) => Terminal.Write(value.ToString());

    public override void Write(char[] buffer, int index, int count)
    {
        if (buffer == null) throw new ArgumentNullException(nameof(buffer));
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
        if (buffer.Length - index < count) throw new ArgumentException("'Count' cannot be larger than a number of available chars.");

        var span = new Span<char>(buffer, index, count);

        Terminal.Write(new string(span));
    }
    // Overriding only these methods ^ should be enough to be a valid Console output. Other overloads inplemented in base class.
}