using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace MTerminal.WPF.ViewModels;

internal class TerminalViewModel : ObservableObject
{
    public CommandsViewModel CommandsViewModel { get; }

    public string Title { get => _title; set { _title = value; OnPropertyChanged(nameof(Title)); } }
    private string _title;

    private Brush _background;
    public Brush Background
    {
        get => _background;
        set
        {
            _background = value;
            _background.Freeze();
            OnPropertyChanged(nameof(Background));
        }
    }

    private SolidColorBrush _foreground;
    public SolidColorBrush Foreground
    {
        get => _foreground;
        set { _foreground = value; _foreground.Freeze(); OnPropertyChanged(nameof(Foreground)); }
    }

    private FontFamily _fontFamily;
    public FontFamily FontFamily
    {
        get => _fontFamily;
        set { _fontFamily = value; OnPropertyChanged(nameof(FontFamily)); }
    }

    private int _bufferCapacity;
    public int BufferCapacity
    {
        get => _bufferCapacity;
        set
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(nameof(value));
            _bufferCapacity = value;
            OnPropertyChanged(nameof(BufferCapacity));
        }
    }

    public TerminalViewModel()
    {
        CommandsViewModel = new CommandsViewModel();

        _title = "MTerminal";
        _background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#161616"));
        _foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ccc"));
        _fontFamily = new FontFamily("Consolas");
        _bufferCapacity = 300;
    }
}