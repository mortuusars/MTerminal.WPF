using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Specialized;

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