using System.Windows.Media;

namespace MTerminal.WPF.Commands;

internal class HelpTerminalCommand : TerminalCommand
{
    public HelpTerminalCommand()
    {
        Command = "help";
        Description = "Shows information about Terminal and available commands.";
        DetailedDescription = "Shows short info if no arguments were entered.\n" +
                              "Specify a command name to see detailed info about it.\n" +
                              "With a '-full' argument detailed info about all commands will be shown.";
        Action = (p) => PrintHelp(p);
    }

    /// <summary>
    /// Prints a message to Terminal Screen that contains info about Terminal and list of all registered commands<br></br>
    /// or detailed info about a command if parameter was specified.
    /// </summary>
    private void PrintHelp(IEnumerable<string> args)
    {
        var commands = Terminal.Commands.RegisteredCommands;

        if (!Terminal.IsNewLine())
            Terminal.WriteLine(); // Start from a new line

        if (!args.Any())
            PrintGeneralHelp(commands);
        else if (args.Contains("-full"))
            PrintCommandsFullInfo(commands);
        else
        {
            // Take all matching commands:
            var matchedCommands = commands.Where(c => c.Command.StartsWith(args.First()));
            if (matchedCommands.Any())
            {
                PrintCommandsFullInfo(matchedCommands);
                return;
            }

            // If none matched - search in their aliases:
            var aliases = commands.Where(c => c.Aliases.Any(a => a.StartsWith(args.First())));
            if (aliases.Any())
            {
                PrintCommandsFullInfo(aliases);
                return;
            }

            Terminal.WriteLine("No matching commands found.");
        }
    }

    /// <summary>Prints some info about Terminal and lists all avalilable commands.</summary>
    private void PrintGeneralHelp(IEnumerable<TerminalCommand> commands)
    {
        Terminal.Write($"MTerminal - {Terminal.Version}", Colors.LightGray);
        Terminal.WriteLine(" - By Mortuus\n", Colors.Gray);

        if (!commands.Any())
        {
            Terminal.WriteLine("No commands have been added.");
            return;
        }

        string longestCommand = commands
            .Select(c => $"{c.Command}  {string.Join(" / ", c.Aliases)}")
            .OrderByDescending(c => c.Length)
            .First();

        int cmdColumnLength = Math.Max(11, longestCommand.Length + 3);

        Terminal.WriteLine("<Command>".PadRight(cmdColumnLength) + "|  <Description - [] is optional parameter>");
        Terminal.WriteLine(new string('_', 40) + "\n"); // Divider

        foreach (var cmd in commands)
        {
            if (cmd.Aliases.Count > 0)
                Terminal.Write($"{cmd.Command} / {string.Join(" / ", cmd.Aliases)}".PadRight(cmdColumnLength), Colors.LightGray);
            else
                Terminal.Write(cmd.Command.PadRight(cmdColumnLength), Colors.LightGray);
            Terminal.WriteLine("|  " + cmd.Description);
        }

        Terminal.WriteLine(new string('_', 40)); // Divider
    }

    /// <summary>Prints detailed info about each command in a collection.</summary>
    private void PrintCommandsFullInfo(IEnumerable<TerminalCommand> commandsToPrint)
    {
        foreach (var cmd in commandsToPrint)
        {
            PrintCommandDetailed(cmd);

            if (commandsToPrint.Count() > 1)
                Terminal.WriteLine(new string('_', 30) + "\n"); // Divider
        }
    }

    /// <summary>Writes a detailed info about command to a Terminal.</summary>
    private static void PrintCommandDetailed(TerminalCommand cmd)
    {
        // Command name and aliases:
        if (cmd.Aliases.Count > 0)
            Terminal.Write($"{cmd.Command} / {string.Join(" / ", cmd.Aliases)}: ", Colors.LightGreen);
        else
            Terminal.Write(cmd.Command + ": ", Colors.LightGreen);

        // Description:
        if (string.IsNullOrWhiteSpace(cmd.Description) && string.IsNullOrWhiteSpace(cmd.DetailedDescription))
        {
            Terminal.WriteLine("\n<No description>", Colors.DarkGray);
            return;
        }

        if (!string.IsNullOrWhiteSpace(cmd.Description))
        {
            Terminal.WriteLine($"{cmd.Description}", Colors.DarkGray);
        }

        // Detailed:
        if (!cmd.DetailedDescription.Equals(string.Empty))
        {
            Terminal.WriteLine("  Details:", Colors.LightGray);
            Terminal.WriteLine($"  {cmd.DetailedDescription}", Colors.DarkGray);
        }
    }
}