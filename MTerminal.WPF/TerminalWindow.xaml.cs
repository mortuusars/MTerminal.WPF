using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MTerminal.WPF;

public partial class TerminalWindow : Window
{
    public bool IsDocked
    {
        get { return (bool)GetValue(IsDockedProperty); }
        set { SetValue(IsDockedProperty, value); }
    }

    public static readonly DependencyProperty IsDockedProperty =
        DependencyProperty.Register("IsDocked", typeof(bool), typeof(TerminalWindow), new PropertyMetadata(false));

    internal int BufferCapacity { get; set; } = 1600;

    internal TerminalWindow()
    {
        InitializeComponent();
        SizeChanged += ConsoleWindow_SizeChanged;
        StateChanged += ConsoleWindow_StateChanged;
    }

    private void Write(string value, SolidColorBrush? brush)
    {
        this.Dispatcher.BeginInvoke(() =>
        {
            if (output.Inlines.Count > BufferCapacity)
                CleanUpInlines();

            Run run = new Run(value);
            if (brush is not null)
                run.Foreground = brush;
            output.Inlines.Add(run);
            ScrollScreenToEnd();
        }, null);
    }

    internal void Write(string value, Color color) => Write(value, new SolidColorBrush(color));
    internal void Write(string value) => Write(value, null);

    internal void ClearScreen()
    {
        this.Dispatcher.Invoke(() => output.Inlines.Clear());
    }

    internal void ClearLastLine()
    {
        this.Dispatcher.Invoke(() =>
        {
            List<Inline> inlines = output.Inlines.ToList();
            inlines.Reverse();
            List<Inline> lastLineElements = new();
            foreach (Inline inline in inlines)
            {
                lastLineElements.Add(inline);
                if (inline is Run run && run.Text.Contains('\n'))
                    break;
            }

            foreach (Inline inline in lastLineElements)
            {
                output.Inlines.Remove(inline);
            }
            ScrollScreenToEnd();
        });
    }

    private void CleanUpInlines()
    {
        var lastItems = output.Inlines.TakeLast(200).ToArray();
        output.Inlines.Clear();
        output.Inlines.AddRange(lastItems);
    }

    private void ScrollScreenToEnd() => screen.ScrollToEnd();

    private Point GetMousePosition()
    {
        var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
        return transform.Transform(Mouse.GetPosition(this));
    }

    private void ConsoleWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (double.IsNaN(Width) || double.IsNaN(Height))
            return;

        IsDocked = WindowState == WindowState.Maximized ||
                  (WindowState == WindowState.Normal &&
                   (Width != RestoreBounds.Width ||
                   Height != RestoreBounds.Height));
    }

    private void ConsoleWindow_StateChanged(object? sender, System.EventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 1;
            IsDocked = true;
            BorderThickness = new Thickness(7);
        }
        else
        {
            BorderThickness = new Thickness(0);
            IsDocked = false;
        }

        commandBox.Focus();
    }

    private void headerBorder_MouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;

        if (e.ClickCount > 1)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            return;
        }
    }

    private void headerBorder_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            e.Handled = true;

            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;

                var mousePos = GetMousePosition();

                this.Left = mousePos.X - this.Width / 2;
                this.Top = mousePos.Y - 36;
            }

            this.DragMove();
        }
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
        this.Close();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        e.Handled = true;

        mainGrid.Height = mainGrid.ActualHeight;
        mainGrid.VerticalAlignment = VerticalAlignment.Bottom;

        TimeSpan duration = TimeSpan.FromMilliseconds(250);

        var anim = new DoubleAnimation(0.0, new Duration(duration), FillBehavior.Stop);
        anim.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut };
        mainGrid.BeginAnimation(Grid.HeightProperty, anim);
        mainGrid.BeginAnimation(Grid.OpacityProperty, anim);

        var timer = new System.Windows.Threading.DispatcherTimer();
        timer.Interval = duration;
        timer.Tick += (s, e) =>
        {
            this.WindowState = WindowState.Minimized;
            mainGrid.VerticalAlignment = VerticalAlignment.Stretch;
            timer.Stop();
        };
        timer.Start();
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
        WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Tab)
        {
            e.Handled = true;
            commandBox.Focus();
        }
    }

    private bool _autoScroll;

    private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        var scroll = (ScrollViewer)sender;

        if (e.ExtentHeightChange == 0)
        {
            if (scroll.VerticalOffset == scroll.ScrollableHeight)
                _autoScroll = true;
            else
                _autoScroll = false;
        }

        if (_autoScroll && e.ExtentHeightChange != 0)
            scroll.ScrollToVerticalOffset(scroll.ExtentHeight);
    }

    private void commandBox_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
    {
        commandBox.ScrollToEnd();
        commandBox.CaretIndex = commandBox.Text.Length;
        ScrollScreenToEnd();
    }

    #region Resize

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    private enum ResizeDirection { Left = 61441, Right = 61442, Top = 61443, Bottom = 61446, BottomRight = 61448, }

    private void StartResize(Visual visual, ResizeDirection direction)
    {
        var hwndSource = (HwndSource)PresentationSource.FromVisual(visual);
        SendMessage(hwndSource.Handle, 0x112, (IntPtr)direction, IntPtr.Zero);
    }

    private void leftResizer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        StartResize((Visual)sender, ResizeDirection.Left);
    }

    private void rightResizer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        StartResize((Visual)sender, ResizeDirection.Right);
    }

    private void bottomResizer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        StartResize((Visual)sender, ResizeDirection.Bottom);
    }

    private void topResizer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        StartResize((Visual)sender, ResizeDirection.Top);
    }

    private void cornerRightResizer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        StartResize((Visual)sender, ResizeDirection.BottomRight);
    }


    #endregion

    private void window_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            if (e.Delta > 0 && screen.FontSize < 64)
                screen.FontSize++;
            else if (e.Delta < 0 && screen.FontSize > 1)
                screen.FontSize--;

            e.Handled = true;
        }
    }
}