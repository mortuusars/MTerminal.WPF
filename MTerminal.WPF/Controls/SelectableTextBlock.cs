using MTerminal.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MTerminal.WPF.Controls;

/// <summary>
/// TextBlock which text can be selected.<br></br><br></br>
/// Thanks 'torvin' from StackOverflow for this solution.
/// </summary>
public class SelectableTextBlock : TextBlock
{
    static SelectableTextBlock()
    {
        FocusableProperty.OverrideMetadata(typeof(SelectableTextBlock), new FrameworkPropertyMetadata(true));

        // Register to allow selectability.
        TextEditorWrapper.RegisterCommandHandlers(typeof(SelectableTextBlock), acceptsRichContent: true, readOnly: true, registerEventListeners: true);

        // Remove the focus rectangle around the control
        FocusVisualStyleProperty.OverrideMetadata(typeof(SelectableTextBlock), new FrameworkPropertyMetadata((object)null!));
    }

    private readonly TextEditorWrapper _editor;

    public SelectableTextBlock()
    {
        // Create TextEditor for this control. Allows selecting the text.
        _editor = TextEditorWrapper.CreateFor(this);
    }

    private readonly Dictionary<Color, SolidColorBrush> _usedColors = new();

    public void Write(string text, Color color)
    {
        Dispatcher.BeginInvoke(() =>
        {
            Run? lastRun = Inlines.LastOrDefault() as Run;

            if (lastRun is not null && ((SolidColorBrush)lastRun.Foreground).Color.Equals(color) && lastRun.Text.Length < 50)
                lastRun.Text += text;
            else
            {
                Run run = new(text);

                if (_usedColors.ContainsKey(color))
                    run.Foreground = _usedColors[color];
                else
                {
                    var brush = new SolidColorBrush(color);
                    brush.Freeze();
                    run.Foreground = brush;
                    _usedColors.Add(color, brush);
                }

                this.Inlines.Add(run);
            }
        });
    }

    public void Write(string text)
    {
        //Write(text, ()Foreground.Color)

        //Dispatcher.BeginInvoke(() =>
        //{
            //Run? lastRun = Inlines.LastOrDefault() as Run;

            //if (lastRun is not null && lastRun.Foreground.Equals(Foreground))
            //    lastRun.Text += text;
            //else
            //{
            //    Run run = new(text);
            //    run.Foreground = Foreground;
            //    this.Inlines.Add(run);
            //}
        //});
    }

    private void Write(string text, Brush brush)
    {
        Dispatcher.BeginInvoke(() =>
        {
            Run? lastRun = Inlines.LastOrDefault() as Run;

            if (lastRun is not null && lastRun.Foreground.Equals(brush))
            {
                if (lastRun.Text.Length < 50)
                    lastRun.Text += text;
                //else
                    

            }

            if (lastRun is not null && lastRun.Foreground.Equals(brush) && lastRun.Text.Length < 50)
                lastRun.Text += text;
            else
            {
                //Run run = new(text);



                //if (_usedColors.ContainsKey(color))
                //    run.Foreground = _usedColors[color];
                //else
                //{
                //    var brush = new SolidColorBrush(color);
                //    brush.Freeze();
                //    run.Foreground = brush;
                //    _usedColors.Add(color, brush);
                //}

                //this.Inlines.Add(run);
            }
        });
    }

    public bool IsNewLine()
    {
        return Dispatcher.Invoke(() => Inlines.LastOrDefault() is Run run && run.Text.EndsWith("\n"));
    }
}