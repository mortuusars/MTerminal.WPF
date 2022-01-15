using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MTerminal.WPF.Windows;

public partial class TerminalWindow : Window
{
    public TerminalWindow()
    {
        InitializeComponent();
        TerminalWindowState.Load(this);
        SizeChanged += (s, e) => RecalculateIsDocked();
        MouseWheel += TerminalWindow_MouseWheel;
        KeyDown += TerminalWindow_KeyDown;
    }


    #region Writing

    public int BufferCapacity { get; set; } = 1500;

    internal void Write(string text, Color? color = null)
    {
        Dispatcher.BeginInvoke(() =>
        {
            if (DisplayOutput.Inlines.Count > BufferCapacity)
                CleanUpInlines();

            var run = new Run(text);

            if (color is not null)
            {
                SolidColorBrush brush = new((Color)color);
                brush.Freeze();
                run.Foreground = brush;
            }

            DisplayOutput.Inlines.Add(run);
            ScrollToEnd();
        });
    }

    internal void ClearScreen() => this.Dispatcher.BeginInvoke(() => DisplayOutput.Inlines.Clear(), null);

    internal void ClearLastLine()
    {
        this.Dispatcher.Invoke(() =>
        {
            Inline[] inlines = DisplayOutput.Inlines.ToArray();
            Array.Reverse(inlines);
            
            List<Inline> lastLineElements = new();
            foreach (Inline inline in inlines)
            {
                lastLineElements.Add(inline);
                if (inline is Run run && run.Text.Contains('\n'))
                    break;
            }

            foreach (Inline inline in lastLineElements)
            {
                DisplayOutput.Inlines.Remove(inline);
            }
            ScrollToEnd();
        });
    }

    private void CleanUpInlines()
    {
        var lastItems = DisplayOutput.Inlines.TakeLast(200).ToArray();
        DisplayOutput.Inlines.Clear();
        DisplayOutput.Inlines.AddRange(lastItems);
    }

    private void ScrollToEnd() => Display.ScrollToEnd();

    #endregion


    #region Window - Buttons, Resizing, etc..

    private void TerminalWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Tab)
        {
            e.Handled = true;
            CommandBox.Focus();
        }
    }

    private void TerminalWindow_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            if (e.Delta > 0) IncreaseFontSize(); //Scroll up
            else if (e.Delta < 0) DecreaseFontSize(); //Scroll down
            e.Handled = true;
        }
    }

    private void IncreaseFontSize()
    {
        if (FontSize < 60)
            this.FontSize += 2;
    }

    private void DecreaseFontSize()
    {
        if (FontSize > 4)
            this.FontSize -= 2;
    }

    public static readonly DependencyProperty IsDockedProperty =
        DependencyProperty.Register(nameof(IsDocked), typeof(bool), typeof(TerminalWindow), new PropertyMetadata(false, OnIsDockedChanged));

    public bool IsDocked
    {
        get { return (bool)GetValue(IsDockedProperty); }
        set { SetValue(IsDockedProperty, value); }
    }

    private void RecalculateIsDocked()
    {
        if (double.IsNaN(Width) || double.IsNaN(Height))
            return;

        IsDocked = WindowState == WindowState.Maximized ||
                  (WindowState == WindowState.Normal &&
                   (Width != RestoreBounds.Width ||
                   Height != RestoreBounds.Height));
    }

    private static void OnIsDockedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        TerminalWindow window = (TerminalWindow)d;

        // Correct header area when snapping and unsnapping window
        if (window.WindowState == WindowState.Maximized)
            window.WindowChromeData.CaptionHeight = window.HeaderRow.Height.Value + window.Root.BorderThickness.Top + 2;
        else
            window.WindowChromeData.CaptionHeight = window.HeaderRow.Height.Value - window.WindowChromeData.ResizeBorderThickness.Top + 2;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        TerminalWindowState.Save(this);
        base.OnClosing(e);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => this.Close();
    private void MinimizeButton_Click(object sender, RoutedEventArgs e) => SetWindowState(WindowState.Minimized, e);
    private void MaximizeButton_Click(object sender, RoutedEventArgs e) => SetWindowState(WindowState.Maximized, e);
    private void RestoreButton_Click(object sender, RoutedEventArgs e) => SetWindowState(WindowState.Normal, e);
    private void SetWindowState(WindowState newState, RoutedEventArgs e)
    {
        e.Handled = true;
        WindowState = newState;
    }

    #endregion
}