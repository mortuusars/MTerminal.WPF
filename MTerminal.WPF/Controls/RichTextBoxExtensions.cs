using System.Windows.Controls;
using System.Windows.Documents;

namespace MTerminal.WPF.Controls;

/// <summary>
/// Contains helpful functions and shortcuts.
/// </summary>
internal static class RichTextBoxExtensions
{
    /// <summary>
    /// Gets pointer to the Document start.
    /// </summary>
    public static TextPointer GetStartPointer(this RichTextBox rtb)
    {
        return rtb.Document.ContentStart;
    }

    /// <summary>
    /// Gets pointer to the Document end.
    /// </summary>
    public static TextPointer GetEndPointer(this RichTextBox rtb)
    {
        return rtb.Document.ContentEnd;
    }

    /// <summary>
    /// Gets the value indicating whether caret is ona new line (next write will be on a new line).
    /// </summary>
    public static bool IsNewLine(this RichTextBox rtb)
    {
        Block lastBlock = rtb.Document.Blocks.LastBlock;
        if (lastBlock is null)
            return true; // No blocks in a document means no text. Ofc it's a new line. 

        TextRange range = new(lastBlock.ContentStart, lastBlock.ContentEnd);
        return range.Text.EndsWith("\n");
    }
}
