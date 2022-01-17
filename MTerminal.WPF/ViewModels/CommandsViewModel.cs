using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MTerminal.WPF.Autocomplete;
using System.Windows.Input;
using System.Windows.Media;

namespace MTerminal.WPF.ViewModels
{
    public class CommandsViewModel : ObservableObject
    {
        public string CommandText { get => _commandText; set { _commandText = value; OnPropertyChanged(nameof(CommandText)); GetAutocompletions(CommandText); } }

        public string AutocompleteSuggestion { get => _autocompleteSuggestion; set { _autocompleteSuggestion = value; OnPropertyChanged(nameof(AutocompleteSuggestion)); } }

        public ICommand ExecuteCommand { get; set; }
        public ICommand AutocompleteCommand { get; set; }

        private string _commandText;

        private Commands _commands;
        private string _autocompleteSuggestion;

        public CommandsViewModel()
        {
            _commands = Terminal.Commands;

            _commandText = string.Empty;
            _autocompleteSuggestion = string.Empty;

            ExecuteCommand = new RelayCommand<string>(command => ExecuteInputCommand((string)command!));
            AutocompleteCommand = new RelayCommand(Autocomplete);
        }

        private void ExecuteInputCommand(string input)
        {
            CommandText = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
                return;

            // Print command that was entered
            string enteredText = Terminal.IsNewLine() ? input : "\n" + input;
            Terminal.WriteLine(enteredText, Colors.Gray);

            int firstSpaceIndex = input.IndexOf(' ');

            string commandString = firstSpaceIndex == -1 ? input : input[..firstSpaceIndex];

            TerminalCommand? command = _commands.Find(commandString);

            if (command is null)
            {
                Terminal.WriteLine($"Command '{commandString}' is unrecognized.", Colors.IndianRed);
                return;
            }

            string parameter = commandString.Equals(input) ? command.DefaultParameter ?? string.Empty : input.Substring(firstSpaceIndex);

            _commands.Execute(command, parameter);
        }

        private void GetAutocompletions(string commandText)
        {
            AutocompleteSuggestion = CommandAutocomplete.MatchOrdered(commandText, _commands.GetCommands()).Command;
        }

        private void Autocomplete()
        {
            CommandText = AutocompleteSuggestion;
            AutocompleteSuggestion = string.Empty;
        }
    }
}
