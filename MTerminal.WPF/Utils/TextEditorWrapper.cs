using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace MTerminal.WPF.Utils;

#pragma warning disable CS8601 // Possible null reference assignment.
/// <summary>
/// Allows use of TextEditor to set properties of a TextBlock (such as selecting).<br></br>
/// TextEditor access is internal for some reason, but required to create simple readonly text control<br></br>
/// that supports formatting and allows selecting the text.<br></br><br></br>
/// This class is copied from StackOverflow.Author: torvin<br></br>
/// Huge thanks for that.
/// </summary>
internal class TextEditorWrapper
{
    private static readonly Type TextEditorType = Type.GetType("System.Windows.Documents.TextEditor, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
    private static readonly PropertyInfo IsReadOnlyProp = TextEditorType!.GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly PropertyInfo TextViewProp = TextEditorType.GetProperty("TextView", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo RegisterMethod = TextEditorType.GetMethod("RegisterCommandHandlers",
        BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(Type), typeof(bool), typeof(bool), typeof(bool) }, null);

    private static readonly Type? TextContainerType = Type.GetType("System.Windows.Documents.ITextContainer, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
    private static readonly PropertyInfo TextContainerTextViewProp = TextContainerType!.GetProperty("TextView");

    private static readonly PropertyInfo TextContainerProp = typeof(TextBlock).GetProperty("TextContainer", BindingFlags.Instance | BindingFlags.NonPublic);

    public static void RegisterCommandHandlers(Type controlType, bool acceptsRichContent, bool readOnly, bool registerEventListeners)
    {
        RegisterMethod.Invoke(null, new object[] { controlType, acceptsRichContent, readOnly, registerEventListeners });
    }

    /// <summary>
    /// Sets TextEditor property that allows specified TextBlock to be selectable.
    /// </summary>
    /// <param name="tb">Target TextBlock.</param>
    /// <exception cref="Exception"/>
    public static TextEditorWrapper CreateFor(TextBlock tb)
    {
        var textContainer = TextContainerProp.GetValue(tb);

        var editor = new TextEditorWrapper(textContainer!, tb, false);
        IsReadOnlyProp.SetValue(editor._editor, true);
        TextViewProp.SetValue(editor._editor, TextContainerTextViewProp.GetValue(textContainer));

        return editor;
    }

    private readonly object _editor;

    public TextEditorWrapper(object textContainer, FrameworkElement uiScope, bool isUndoEnabled)
    {
        _editor = Activator.CreateInstance(TextEditorType, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance,
            null, new[] { textContainer, uiScope, isUndoEnabled }, null)!;
    }
}
#pragma warning restore CS8601 // Possible null reference assignment.
