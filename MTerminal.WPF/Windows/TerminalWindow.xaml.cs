using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MTerminal.WPF.Windows;

public partial class TerminalWindow : Window, IOutput
{
    public TerminalWindow()
    {
        InitializeComponent();

        Loaded += (s, e) =>
        {
            TerminalWindowState.Load(this);
            //if (this.KeepWrittenText)
                //ConsoleOutput.LoadXamlDocument("doc.xaml");
        };
        SizeChanged += (s, e) => RecalculateIsDocked();
        PreviewMouseWheel += TerminalWindow_PreviewMouseWheel;
        KeyDown += TerminalWindow_KeyDown;
    }

    #region Writing

    /// <summary>
    /// Gets all text that is currently displayed in a console.
    /// </summary>
    public string Text { get => ConsoleOutput.GetText(); }

    /// <summary>
    /// Maximum amount lines in a terminal.
    /// </summary>
    public int BufferCapacity { get; set; } = 300;

    /// <summary>Indicates that written text should be saved and restored on window reopen.</summary>
    //public bool KeepWrittenText { get => _keepWrittenText; set => _keepWrittenText = value; }
    //private bool _keepWrittenText = true;

    public void Write(string text) => ConsoleOutput.Write(text);
    public void Write(string text, Color color) => ConsoleOutput.Write(text, color);
    public bool IsNewLine() => ConsoleOutput.IsNewLine();
    public void Clear() => ConsoleOutput.Clear();
    public void RemoveLastLine() => ConsoleOutput.RemoveLastLine();

    #endregion

    #region Window - Buttons, Resizing, etc..

    /// <summary>Identifies the <see cref="TerminalWindow.IsDocked"/> dependency property.</summary>
    public static readonly DependencyProperty IsDockedProperty =
        DependencyProperty.Register(nameof(IsDocked), typeof(bool), typeof(TerminalWindow), new PropertyMetadata(false, OnIsDockedChanged));

    /// <summary>Indicates whether the window is docked to one of the sides of a screen or maximized.</summary>
    public bool IsDocked
    {
        get { return (bool)GetValue(IsDockedProperty); }
        set { SetValue(IsDockedProperty, value); }
    }

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
            e.Handled = true;
        }
    }

    // PreviewMouseWheel to handle wheel event before console's ScrollViewer.
    private void TerminalWindow_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        // Change font size on Ctrl + MouseWheel:
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

        // Adjust header area when snapping and unsnapping window
        if (window.WindowState == WindowState.Maximized)
            window.WindowChromeData.CaptionHeight = window.HeaderRow.Height.Value + window.Root.BorderThickness.Top + 2;
        else
            window.WindowChromeData.CaptionHeight = window.HeaderRow.Height.Value - window.WindowChromeData.ResizeBorderThickness.Top + 2;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        TerminalWindowState.Save(this);
        //if (this.KeepWrittenText)
            //ConsoleOutput.SaveXamlDocument("doc.xaml");
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