using System.IO;
using System.Text;

namespace MTerminal.WPF;

/// <summary>
/// Used to replace output of a <see cref="Console"/>.
/// </summary>
public class TerminalWriter : TextWriter
{
    /// <summary>Encoding of the text writer.</summary>
    public override Encoding Encoding => _encoding;
    private Encoding _encoding = Encoding.Default;
    /// <summary>Sets the Encoding.</summary>
    /// <param name="encoding"></param>
    public void SetEncoding(Encoding encoding) => _encoding = encoding;

    /// <summary>Writes a new line separator to the Terminal.</summary>
    public override void WriteLine() => Terminal.Write(new string(CoreNewLine));
    /// <summary>Writes a character to the terminal.</summary>
    /// <param name="value">Character to write.</param>
    public override void Write(char value) => Terminal.Write(value.ToString());
    /// <summary>Writes a buffer of chars from the specified index and with specified count to the Terminal.</summary>
    /// <exception cref="ArgumentNullException">When the buffer is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If index or count is less than 0.</exception>
    /// <exception cref="ArgumentException">If count is bigger than length of the buffer.</exception>
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