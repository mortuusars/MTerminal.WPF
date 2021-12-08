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
    public string WindowTitle { get => _windowTitle; set { _windowTitle = value; OnPropertyChanged(nameof(WindowTitle)); } }
    public ushort MaxWritesCount { get; set; } = 2000;

    public ObservableCollection<Inline> Inlines { get; set; }

    public ICommand ExecuteCommand { get; }

    private string _windowTitle;

    public TerminalViewModel()
    {
        _windowTitle = "MTerminal";

        Inlines = new ObservableCollection<Inline>();

        ExecuteCommand = new RelayCommand(_ => { });
    }

    internal void WriteLine(string message, SolidColorBrush color)
    {
        MaintainCollection();

        if (Inlines.Count == 0)
            Write(message, color);
        else
            Inlines.Add(new Run(Environment.NewLine + message) { Foreground = color });
    }

    internal void Write(string message, SolidColorBrush color)
    {
        MaintainCollection();
        Inlines.Add(new Run(message) { Foreground = color });
    }

    internal void ReplaceLastLine(string message, SolidColorBrush color)
    {
        MaintainCollection();
        for (int i = Inlines.Count - 1; i > 0; i--)
        {
            if (Inlines[i] is not Run run)
                continue;

            if (!run.Text.StartsWith(Environment.NewLine))
                Inlines.RemoveAt(i);
            else
                break;
        }

        Inlines.RemoveAt(Inlines.Count - 1);
        Inlines.Add(new Run(Environment.NewLine + message) { Foreground = color });
    }

    internal void ClearScreen()
    {
        Inlines.Clear();
    }

    private void MaintainCollection()
    {
        if (Inlines.Count > MaxWritesCount)
            Inlines.RemoveAt(0);
    }
}