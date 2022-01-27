using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MTerminal.WPF.Autocomplete;
using MTerminal.WPF.Commands;
using MTerminal.WPF.Utils;
using System.Windows.Input;
using System.Windows.Media;

namespace MTerminal.WPF.ViewModels;

internal class CommandsViewModel : ObservableObject
{
    // Window sets this when it's in read mode. Indicates that we should stop suggesting autocomplete and such.
    public bool IsReadingInput { get; set; }

    /// <summary>
    /// Text that user types in a command text block.
    /// </summary>
    public string CommandText
    {
        get => _commandText;
        set { _commandText = value; OnPropertyChanged(nameof(CommandText)); GetAutocompletionsAsync(CommandText); }
    }
    private string _commandText;

    /// <summary>
    /// Suggestion of the available autocompletion.
    /// </summary>
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

    private readonly History<string> _history;

    public CommandsViewModel()
    {
        _commands = Terminal.Commands;
        _commandInputProcessor = new CommandInputProcessor(_commands);

        _commandText = string.Empty;
        _autocompleteSuggestion = string.Empty;
        _history = new History<string>(20);

        ExecuteCommand = new RelayCommand<string>(command => ParseAndExecute(command));
        AutocompleteCommand = new RelayCommand(Autocomplete);
        AutocompleteBackwardsCommand = new RelayCommand(AutocompleteBackwards);

        HistoryUpCommand = new RelayCommand(() => CommandText = _history.GetPrevious());
        HistoryDownCommand = new RelayCommand(() => CommandText = _history.GetNext());
    }

    /// <summary>
    /// Finds a command from the user input, and executes it.
    /// </summary>
    /// <param name="input">Input to parse and execute.</param>
    private void ParseAndExecute(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return;

        CommandText = string.Empty;
        _history.Append(input);

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

        Execute(command, input, args.ToArray());
    }

    /// <summary>
    /// Executes entered command with args.
    /// </summary>
    /// <param name="command">Command to execute.</param>
    /// <param name="input">Entered text to print if exception is thrown when executing.</param>
    /// <param name="args">Args to pass to the command.</param>
    private void Execute(TerminalCommand command, string input, string[] args)
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

    /// <summary>
    /// Sets autocomplete suggestion to the matched command.
    /// </summary>
    /// <param name="commandText"></param>
    private async void GetAutocompletionsAsync(string commandText)
    {
        if (IsReadingInput)
            return;

        await Task.Run(() => AutocompleteSuggestion = CommandAutocomplete.MatchOrdered(commandText, _commands.CommandNames));
    }

    /// <summary>
    /// Replaces entered command text with autocomplete suggestion or next command in alphabetical order if used successively.
    /// </summary>
    private void Autocomplete()
    {
        if (CommandText.Equals(AutocompleteSuggestion, StringComparison.InvariantCultureIgnoreCase))
        {
            var commands = _commands.RegisteredCommands;

            var next = CommandAutocomplete.GetNextOrdered(CommandText, commands.Select(c => c.Command));
            if (next.Length == 0)
                next = commands.Select(c => c.Command).OrderBy(c => c).FirstOrDefault(string.Empty);

            CommandText = next;
        }
        else
        {
            CommandText = AutocompleteSuggestion;
        }
    }

    /// <summary>
    /// Replaces entered command text with previous command in alphabetical order.
    /// </summary>
    private void AutocompleteBackwards()
    {
        CommandText = CommandAutocomplete.GetPreviousOrdered(CommandText, _commands.RegisteredCommands.Select(c => c.Command));
    }
}
