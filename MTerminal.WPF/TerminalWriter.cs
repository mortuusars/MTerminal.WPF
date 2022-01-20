﻿using System.IO;
using System.Text;
using System.Windows.Media;

namespace MTerminal.WPF;

public class TerminalWriter : TextWriter
{
    public override Encoding Encoding => _encoding;
    private Encoding _encoding = Encoding.Default;
    public void SetEncoding(Encoding encoding) => _encoding = encoding;

    /// <summary>
    /// Destination of the writer.
    /// </summary>
    public IOutput? WriterOutput { get; set; }

    /// <summary>
    /// Creates a new instance of the Writer with a writer output to write to.
    /// </summary>
    /// <param name="writerOutput"></param>
    public TerminalWriter(IOutput writerOutput)
    {
        WriterOutput = writerOutput;
    }

    /// <summary>
    /// Creates a new instance of the Writer.
    /// </summary>
    public TerminalWriter() { }

    /// <summary>
    /// Writes text of the specified color.
    /// </summary>
    /// <param name="text">Text to write.</param>
    /// <param name="color">Color of the text.</param>
    public void Write(string value, Color color)
    {
        WriterOutput?.Write(value, color);
    }

    /// <summary>
    /// Writes text of the specified color and moves caret on a new line.
    /// </summary>
    /// <param name="text">Text to write.</param>
    /// <param name="color">Color of the text.</param>
    public void WriteLine(string value, Color color)
    {
        Write(value, color);
        WriteLine();
    }

    // Overriding only these methods should be enough to be a valid Console output. Other overloads inplemented in base class.

    public override void WriteLine() => WriterOutput?.Write(new string(CoreNewLine));

    public override void Write(char value) => WriterOutput?.Write(value.ToString());

    public override void Write(char[] buffer, int index, int count)
    {
        if (buffer == null) throw new ArgumentNullException("buffer");
        if (index < 0) throw new ArgumentOutOfRangeException("index");
        if (count < 0) throw new ArgumentOutOfRangeException("count");
        if (buffer.Length - index < count) throw new ArgumentException("'Count' cannot be larger than a number of available chars.");

        var span = new Span<char>(buffer, index, count);

        WriterOutput?.Write(new string(span));
    }
}