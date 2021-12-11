﻿using MTerminal.WPF.Autocomplete;
using System.Windows.Media;

namespace MTerminal.WPF;
public class Commands
{
    internal List<TerminalCommand> CommandsList { get; }

    public Commands()
    {
        CommandsList = new List<TerminalCommand>();

        Add(new TerminalCommand("help")
        {
            Description = "['command'] | [-full] | Prints information about commands and Terminal.",
            DetailedDescription = "Prints some info about Terminal and lists all registered commands.\n" +
            "['command']: When followed up with a string - prints detailed info about commands which names start with that string\n" +
            "[-full]: When paired with a '-full' parameter - will print detailed info about all registered commands.",
            Action = (p) => PrintHelp(p),
        });

        Add(new TerminalCommand("clear")
        {
            Description = "Clears the Terminal screen.",
            Action = (_) => Terminal.Clear()
        });
    }

    public Commands Add(TerminalCommand command)
    {
        if (Contains(command))
            Terminal.WriteLine($"\n[Add Command] {command.Command} already exists. New command would not be added.", Colors.IndianRed);
        else
            CommandsList.Add(command);

        return this;
    }

    public TerminalCommand? Find(string command) => CommandsList.FirstOrDefault(c => c.Command.Equals(command, StringComparison.InvariantCultureIgnoreCase));
    public bool Remove(TerminalCommand command) => CommandsList.Remove(command);
    public bool Remove(string command) => Find(command) is TerminalCommand cmd ? CommandsList.Remove(cmd) : false;
    public bool Contains(TerminalCommand command) => Find(command.Command) is not null;
    public bool Contains(string command) => Find(command) is not null;
    public IEnumerable<TerminalCommand> GetCommands() => CommandsList;

    internal void Execute(TerminalCommand command, string parameter)
    {
        try
        {
            command.Execute(parameter);
        }
        catch (Exception ex)
        {
            Terminal.WriteLine($"'{command}' has thrown an exception:", Color.FromRgb(245, 65, 65));
            Terminal.WriteLine(ex, Colors.IndianRed);
        }
    }

    internal void Execute(string command, string parameter)
    {
        TerminalCommand? cmd = Find(command);
        if (cmd is null)
        {
            Terminal.WriteLine($"\nCommand '{command}' not found.", Colors.IndianRed);
            return;
        }

        Execute(cmd, parameter);
    }

    /// <summary>
    /// Prints a message to Terminal Screen that contains info about Terminal and list of all registered commands or detailed info about a command if parameter was specified.
    /// </summary>
    private void PrintHelp(string parameteters)
    {
        Terminal.WriteLine();

        if (string.IsNullOrWhiteSpace(parameteters))
            PrintGeneralHelp();
        else if (parameteters.Contains("-full"))
            PrintCommandsFullInfo(CommandsList);
        else
            PrintCommandsFullInfo(CommandAutocomplete.MatchAll(parameteters, CommandsList));
    }

    private void PrintGeneralHelp()
    {
        Terminal.Write($"MTerminal - {Terminal.Version}", Colors.LightGray);
        Terminal.WriteLine(" - By Mortuus\n", Colors.Gray);

        if (CommandsList.Count == 0)
        {
            Terminal.WriteLine("No commands have been registered.");
            return;
        }

        int cmdColumnLength = Math.Max(11, CommandsList.OrderByDescending(c => c.Command.Length).First().Command.Length + 2);

        Terminal.WriteLine("<Command>".PadRight(cmdColumnLength) + "|  <Description - [] is optional parameter>");
        Terminal.WriteLine(new string('_', 40) + "\n"); // Divider

        foreach (var cmd in CommandsList)
        {
            Terminal.Write(cmd.Command.PadRight(cmdColumnLength), Colors.LightGray);
            Terminal.WriteLine("|  " + cmd.Description);
        }

        Terminal.WriteLine(new string('_', 40)); // Divider
    }

    private void PrintCommandsFullInfo(IEnumerable<TerminalCommand> commandsToPrint)
    {
        if (commandsToPrint.Count() == 0)
            Terminal.WriteLine($"No matching commands found.");
        else
        {
            foreach (var cmd in commandsToPrint)
            {
                Terminal.WriteLine(cmd.Command + ":\n", Colors.LightGreen);

                Terminal.WriteLine("Description:");
                Terminal.WriteLine(cmd.Description, Colors.LightGray);

                Terminal.WriteLine();

                if (!cmd.DetailedDescription.Equals(string.Empty))
                {
                    Terminal.WriteLine("Detailed Desctiption:");
                    Terminal.WriteLine(cmd.DetailedDescription, Colors.LightGray);
                    Terminal.WriteLine();
                }

                if (!cmd.DefaultParameter.Equals(string.Empty))
                {
                    Terminal.WriteLine("Default parameter:");
                    Terminal.WriteLine(cmd.DefaultParameter, Colors.LightGray);
                    Terminal.WriteLine();
                }

                if (commandsToPrint.Count() > 1)
                    Terminal.WriteLine(new string('_', 30) + "\n"); // Divider
            }
        }
    }
}