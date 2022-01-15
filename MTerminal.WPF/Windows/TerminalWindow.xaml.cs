using System.ComponentModel;
using System.Windows;

namespace MTerminal.WPF.Windows;

public enum WindowClosingBehavior
{
    Close,
    Hide
}

public partial class TerminalWindow : Window
{
    public WindowClosingBehavior ClosingBehavior { get; set; }

    public TerminalWindow()
    {
        InitializeComponent();
        TerminalWindowState.Load(this);

        SizeChanged += (s, e) => RecalculateIsDocked();
    }

    public void Close(WindowClosingBehavior closingBehavior)
    {
        ClosingBehavior = closingBehavior;
        Close();
    }

    #region Window Control - Buttons, Resizing, etc..

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

        // Corners does not look good if window is docked
        if (window.IsDocked)
            window.WindowChromeData.CornerRadius = new CornerRadius(0);
        else
            window.WindowChromeData.CornerRadius = new CornerRadius(18);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        TerminalWindowState.Save(this);

        if (ClosingBehavior == WindowClosingBehavior.Hide)
        {
            e.Cancel = true;
            this.Hide();
        }

        base.OnClosing(e);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => this.Close(ClosingBehavior);
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