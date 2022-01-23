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
}
