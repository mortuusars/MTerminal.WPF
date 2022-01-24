using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MTerminal.WPF.Autocomplete;
using System.Windows.Input;
using System.Windows.Media;
using MTerminal.WPF.Commands;
using MTerminal.WPF.Utils;

namespace MTerminal.WPF.ViewModels
{
    public class CommandsViewModel : ObservableObject
    {
        public string CommandText
        {
            get => _commandText;
            set { _commandText = value; OnPropertyChanged(nameof(CommandText)); GetAutocompletionsAsync(CommandText); }
        }
        private string _commandText;

        public string AutocompleteSuggestion
        {
            get => _autocompleteSuggestion;
            set { _autocompleteSuggestion = value; OnPropertyChanged(nameof(AutocompleteSuggestion)); }
        }
        private string _autocompleteSuggestion;

        public ICommand ExecuteCommand { get; }
        public ICommand AutocompleteCommand { get; }
        public ICommand AutocompleteBackwardsCommand { get; }
        public ICommand HistoryUpCommand { get; }
        public ICommand HistoryDownCommand { get; }

        private readonly CommandRegistry _commands;
        private readonly CommandInputProcessor _commandInputProcessor;

        private History<string> _history;

        public CommandsViewModel()
        {
            _commands = Terminal.Commands;
            _commandInputProcessor = new CommandInputProcessor(_commands);

            _commandText = string.Empty;
            _autocompleteSuggestion = string.Empty;
            _history = new History<string>(20);

            ExecuteCommand = new RelayCommand<string>(command => ParseAndExecute(command!));
            AutocompleteCommand = new RelayCommand(Autocomplete);
            AutocompleteBackwardsCommand = new RelayCommand(AutocompleteBackwards);

            HistoryUpCommand = new RelayCommand(() => CommandText = _history.GetPrevious());
            HistoryDownCommand = new RelayCommand(() => CommandText = _history.GetNext());
        }

        private void ParseAndExecute(string input)
        {
            CommandText = string.Empty;
            _history.Append(input);

            if (string.IsNullOrWhiteSpace(input))
                return;

            //Print entered text:
            string enteredText = Terminal.IsNewLine() ? input : "\n" + input;
            Terminal.WriteLine(enteredText, Colors.Gray);

            // Parse input:
            (TerminalCommand command, IEnumerable<string> args) = _commandInputProcessor.Process(input);

            if (command.IsEmpty())
            {
                Terminal.WriteLine($"Command '{input}' is unrecognized.", Colors.IndianRed);
                return;
            }

            Execute(input, command, args);
        }

        private void Execute(string input, TerminalCommand command, IEnumerable<string> args)
        {
            try
            {
                command.Execute(args);
            }
            catch (Exception ex)
            {
                // Printing 'input' instead of command name because of aliases. And it shows args, which is nice too.
                Terminal.WriteLine($"'{input}' has thrown an exception:", Color.FromRgb(245, 65, 65));
                Terminal.WriteLine(ex, Colors.IndianRed);

                if (command.ThrowsException)
                    throw;
            }
        }

        private async void GetAutocompletionsAsync(string commandText)
        {
            await Task.Run(() => AutocompleteSuggestion = CommandAutocomplete.MatchOrdered(commandText, _commands.RegisteredCommands.Keys));
        }

        private void Autocomplete()
        {
            if (CommandText == AutocompleteSuggestion)
            {
                var next = CommandAutocomplete.GetNextOrdered(CommandText, _commands.GetCommands().Select(c => c.Command));
                if (next.Length == 0)
                    next = _commands.GetCommands().Select(c => c.Command).OrderBy(c => c).FirstOrDefault(string.Empty);
                CommandText = next;
            }
            else
            {
                CommandText = AutocompleteSuggestion;
            }
        }

        private void AutocompleteBackwards()
        {
            CommandText = CommandAutocomplete.GetPreviousOrdered(CommandText, _commands.GetCommands().Select(c => c.Command));
        }
    }
}
