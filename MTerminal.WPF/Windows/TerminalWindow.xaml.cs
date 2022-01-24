using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace MTerminal.WPF.Windows;

public partial class TerminalWindow : Window
{
    public TerminalWindow()
    {
        InitializeComponent();

        Loaded += TerminalWindow_Loaded;
        Activated += (s, e) => Keyboard.Focus(CommandBox);
        SizeChanged += (s, e) => RecalculateIsDocked();
        PreviewMouseWheel += TerminalWindow_PreviewMouseWheel;
        PreviewKeyDown += TerminalWindow_PreviewKeyDown;

        CommandRow.MouseLeftButtonDown += (s, e) => { Keyboard.Focus(CommandBox); e.Handled = true; };
        CommandBox.TargetUpdated += (s, e) => CommandBox.CaretIndex = int.MaxValue;

    }

    private void TerminalWindow_Loaded(object sender, RoutedEventArgs e)
    {
        TerminalWindowState.Load(this);

        // Bind to view model property, it's easier than setting value manually every time window is created (and on every change too).
        var binding = new Binding("BufferCapacity");
        binding.Source = DataContext;
        BindingOperations.SetBinding(this, BufferCapacityProperty, binding);
    }

    #region Writing

    /// <summary>
    /// Gets all text that is currently displayed in a console.
    /// </summary>
    public string Text { get => ConsoleOutput.GetText(); }

    public static readonly DependencyProperty BufferCapacityProperty =
        DependencyProperty.Register(nameof(BufferCapacity), typeof(int), typeof(TerminalWindow), new PropertyMetadata(300, OnBufferCapacityChanged));

    private static void OnBufferCapacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var window = (TerminalWindow)d;
        window.ConsoleOutput.BufferCapacity = (int)e.NewValue;
    }

    public int BufferCapacity
    {
        get { return (int)GetValue(BufferCapacityProperty); }
        set { SetValue(BufferCapacityProperty, value); }
    }

    public void Write(string text) => ConsoleOutput.Write(text);
    public void Write(string text, Color color) => ConsoleOutput.Write(text, color);
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

    private void TerminalWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Tab && !CommandBox.IsFocused)
        {
            e.Handled = true;
            CommandBox.Focus();
        }
        else if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            if (e.Key == Key.OemPlus)
            {
                IncreaseFontSize();
                e.Handled = true;
            }
            else if (e.Key == Key.OemMinus)
            {
                DecreaseFontSize();
                e.Handled = true;
            }
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
        if (FontSize < 64)
            this.FontSize += 2;
    }

    private void DecreaseFontSize()
    {
        if (FontSize > 12)
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