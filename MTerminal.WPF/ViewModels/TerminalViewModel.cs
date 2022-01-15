using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace MTerminal.WPF.ViewModels;

internal class TerminalViewModel : ObservableObject
{
    public CommandViewModel CommandViewModel { get; set; }

    public string WindowTitle { get => _windowTitle; set { _windowTitle = value; OnPropertyChanged(nameof(WindowTitle)); } }
    public SolidColorBrush DefaultForeground { get => _defaultForeground; set { _defaultForeground = value; _defaultForeground.Freeze(); OnPropertyChanged(nameof(DefaultForeground)); } }

    private string _windowTitle;
    private SolidColorBrush _defaultForeground;

    public TerminalViewModel(CommandViewModel commandViewModel)
    {
        CommandViewModel = commandViewModel;

        _windowTitle = "MTerminal";
        _defaultForeground = Brushes.DarkGray;
        _defaultForeground.Freeze();
    }
}