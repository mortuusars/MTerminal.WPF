using Microsoft.Toolkit.Mvvm.ComponentModel;
using MTerminal.WPF.Autocomplete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace MTerminal.WPF.ViewModels
{
    internal class CommandViewModel : ObservableObject
    {
        public string CommandText { get => _commandText; set { _commandText = value; OnPropertyChanged(nameof(CommandText)); GetAutocompletions(CommandText); } }

        public string AutocompleteSuggestion { get => _autocompleteSuggestion; set { _autocompleteSuggestion = value; OnPropertyChanged(nameof(AutocompleteSuggestion)); } }

        public ICommand ExecuteCommand { get; set; }
        public ICommand AutocompleteCommand { get; set; }

        private string _commandText;

        private Commands _commands;
        private string _autocompleteSuggestion;

        public CommandViewModel(Commands commands)
        {
            _commands = commands;

            _commandText = string.Empty;
            _autocompleteSuggestion = string.Empty;

            ExecuteCommand = new RelayCommand(command => ExecuteInputCommand((string)command));
            AutocompleteCommand = new RelayCommand(command => Autocomplete((string)command));
        }               

        private void ExecuteInputCommand(string input)
        {
            CommandText = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
                return;

            Terminal.WriteLine("\n" + input, Colors.Gray);

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

        private void Autocomplete(string command)
        {
            CommandText = AutocompleteSuggestion;
            AutocompleteSuggestion = string.Empty;
        }
    }
}
