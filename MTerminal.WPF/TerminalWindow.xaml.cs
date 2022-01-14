using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MTerminal.WPF;

internal class TerminalWindowState
{
    public double Width { get; set; } = 850;
    public double Height { get; set; } = 450;
    public double Left { get; set; } = 200;
    public double Top { get; set; } = 200;

    public double FontSize { get; set; } = 16;
    public WindowState WindowState { get; set; }
}

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

    private string? _stateFilePath = null;
    private const string _folderName = "MTerminal";

    internal TerminalWindow()
    {
        InitializeComponent();
        SizeChanged += ConsoleWindow_SizeChanged;
        StateChanged += ConsoleWindow_StateChanged;
        LocationChanged += (_, _) => SaveWindowState();

        Loaded += TerminalWindow_Loaded;
    }

    private void TerminalWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string terminalFolder = Path.Combine(localFolder, _folderName);
            Directory.CreateDirectory(terminalFolder);

            var appName = Process.GetCurrentProcess().ProcessName;
            _stateFilePath = Path.Combine(terminalFolder, appName + ".json");
            if (File.Exists(_stateFilePath))
            {
                string json = File.ReadAllText(_stateFilePath);
                var state = JsonSerializer.Deserialize<TerminalWindowState>(json);

                if (state is null)
                    return;

                this.Width = state.Width;
                this.Height = state.Height;
                this.Left = state.Left;
                this.Top = state.Top;
                this.FontSize = state.FontSize;
                this.WindowState = state.WindowState;
            }
        }
        catch (Exception ex)
        {
            Terminal.WriteLine($"\nCannot restore terminal window settings: '{ ex.Message}'", Colors.DarkRed);
        }
    }

    private async void SaveWindowState()
    {
        try
        {
            TerminalWindowState state = new()
            {
                Width = this.Width,
                Height = this.Height,
                Left = this.Left,
                Top = this.Top,
                FontSize = this.FontSize,
                WindowState = this.WindowState
            };
            string json = JsonSerializer.Serialize(state, new JsonSerializerOptions() { WriteIndented = true });

            if (_stateFilePath is not null)
                await File.WriteAllTextAsync(_stateFilePath, json);
        }
        catch (Exception ex)
        {
            Terminal.WriteLine($"\nFailed to save terminal window settings: '{ex.Message}'", Colors.DarkRed);
        }
    }

    internal void Write(string text, Color? color = null)
    {
        Dispatcher.BeginInvoke(() =>
        {
            if (output.Inlines.Count > BufferCapacity)
                CleanUpInlines();

            var run = new Run(text);

            if (color is not null)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)color);
                brush.Freeze();
                run.Foreground = brush;
            }

            output.Inlines.Add(run);
            ScrollScreenToEnd();
        });
    }

    internal void ClearScreen() => this.Dispatcher.BeginInvoke(() => output.Inlines.Clear(), null);

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

    private void commandBox_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
    {
        commandBox.ScrollToEnd();
        commandBox.CaretIndex = commandBox.Text.Length;
        ScrollScreenToEnd();
    }

    #region WindowControl

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

        SaveWindowState();
    }

    private void ConsoleWindow_StateChanged(object? sender, System.EventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 1;
            IsDocked = true;
            BorderThickness = new Thickness(7);
            SaveWindowState();
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
        Terminal.CloseWindow();
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

    private void window_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            if (e.Delta > 0 && screen.FontSize < 64)
                IncreaseFontSize();
            else if (e.Delta < 0 && screen.FontSize > 1)
                DecreaseFontSize();

            SaveWindowState();
            e.Handled = true;
        }
    }

    private void IncreaseFontSize() => this.FontSize++;

    private void DecreaseFontSize() => this.FontSize--;

    #endregion


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

}