using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MTerminal.WPF.Windows;

public partial class TerminalWindow : Window, IWriterOutput
{
    public static readonly DependencyProperty AutoScrollOnChangeProperty =
        DependencyProperty.Register(nameof(AutoScrollOnChange), typeof(bool), typeof(TerminalWindow), new PropertyMetadata(true));

    public static readonly DependencyProperty IsCurrentlyScrollableProperty =
        DependencyProperty.Register(nameof(IsCurrentlyScrollable), typeof(bool), typeof(TerminalWindow), new PropertyMetadata(false));

    public bool IsCurrentlyScrollable
    {
        get { return (bool)GetValue(IsCurrentlyScrollableProperty); }
        set { SetValue(IsCurrentlyScrollableProperty, value); }
    }

    public bool AutoScrollOnChange
    {
        get { return (bool)GetValue(AutoScrollOnChangeProperty); }
        set { SetValue(AutoScrollOnChangeProperty, value); }
    }

    public TerminalWindow()
    {
        InitializeComponent();

        Loaded += (s, e) => TerminalWindowState.Load(this);
        SizeChanged += (s, e) => RecalculateIsDocked();
        StateChanged += TerminalWindow_StateChanged;
        MouseWheel += TerminalWindow_MouseWheel;
        KeyDown += TerminalWindow_KeyDown;

        var timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(300);
        timer.Tick += WorkingTimer_Tick;
        timer.Start();
    }

    private void WorkingTimer_Tick(object? sender, EventArgs e)
    {
        if (_scrollToEndQueued && AutoScrollOnChange)
        {
            OutputScrollViewer.ScrollToEnd();
            _scrollToEndQueued = false;
        }

        IsCurrentlyScrollable = OutputScrollViewer.ScrollableHeight > 0.01;
    }

    #region Writing

    public int BufferCapacity { get; set; } = 1500;

    private bool _scrollToEndQueued;

    public void Write(string text)
    {
        //if (Output.Inlines.Count > BufferCapacity)
        //CleanUpInlines();

        Output.Write(text);
        _scrollToEndQueued = true;
    }

    public void Write(string text, Color color)
    {
        //if (Output.Inlines.Count > BufferCapacity)
        //CleanUpInlines();

        Output.Write(text, color);
        _scrollToEndQueued = true;
    }

    public bool IsNewLine() => Output.IsNewLine();

    public void ClearScreen() => Dispatcher.BeginInvoke(() => Output.Inlines.Clear(), null);

    public void ClearLastLine()
    {
        this.Dispatcher.Invoke(() =>
        {
            Inline[] inlines = Output.Inlines.ToArray();
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
                Output.Inlines.Remove(inline);
            }
            _scrollToEndQueued = true;
        });
    }

    internal IEnumerable<Inline> GetInlines() => Output.Inlines.ToArray();
    internal void SetInlines(IEnumerable<Inline> inlines) => Output.Inlines.AddRange(inlines);

    #endregion

    #region Window - Buttons, Resizing, etc..

    private void TerminalWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Tab)
        {
            e.Handled = true;
            CommandBox.Focus();
        }
        else if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            if (e.Key == Key.OemPlus) IncreaseFontSize();
            else if (e.Key == Key.OemMinus) DecreaseFontSize();
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

    private void TerminalWindow_StateChanged(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Maximized)
            this.WindowChromeData.GlassFrameThickness = new Thickness(20);
        else
            this.WindowChromeData.GlassFrameThickness = new Thickness(0, 0, 0, 1);
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