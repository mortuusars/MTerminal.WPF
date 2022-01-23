using System.Text;

namespace MTerminal.WPF.Commands;

/// <summary>
/// Parses command input to find matching command and arguments that were passsed in.
/// </summary>
internal class CommandInputProcessor
{
    private readonly CommandRegistry _terminalCommands;

    public CommandInputProcessor(CommandRegistry terminalCommands)
    {
        _terminalCommands = terminalCommands;
    }

    /// <summary>
    /// Finds matching command, and parses arguments to a collection.<br></br>
    /// Arguments are splitted by space. To include spaces in a single string - surround it with quotes.
    /// </summary>
    /// <param name="input">Input string to process.</param>
    /// <returns>Matched command and passed arguments.</returns>
    public (TerminalCommand command, IEnumerable<string> args) Process(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return (TerminalCommand.Empty, Array.Empty<string>());

        var span = input.Trim().AsSpan();

        int firstSpaceIndex = span.IndexOf(' ');

        string commandName = firstSpaceIndex == -1 ? span.ToString() : span.Slice(0, span.IndexOf(' ')).ToString();

        TerminalCommand? command = _terminalCommands.Find(commandName);
        if (command is null)
            return (TerminalCommand.Empty, Array.Empty<string>());

        if (commandName.Length + 1 >= span.Length)
            return (command, Array.Empty<string>());

        ReadOnlySpan<char> argsPart = span.Slice(commandName.Length + 1);
        IEnumerable<string> args = ParseArgs(argsPart);
        return (command, args);
    }

    private IEnumerable<string> ParseArgs(ReadOnlySpan<char> input)
    {
        // If no quotes in an input - just split it by space:
        if (input.IndexOf('\"') == -1 && input.IndexOf('\'') == -1)
            return input.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());

        // Otherwise build args one-by-one:
        List<string> args = new();
        StringBuilder builder = new();
        bool isQuotedPart = false;

        // If part of the input surrounded by quotes - spaces are included in an arg.
        // If not - spaces act as args split.
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i].Equals(' '))
            {
                if (isQuotedPart)
                    builder.Append(' ');
                else if (builder.Length > 0)
                {
                    args.Add(builder.ToString());
                    builder.Clear();
                }
            }
            else if (input[i].Equals('\"') || input[i].Equals('\''))
            {
                if (!isQuotedPart)
                    isQuotedPart = true;
                else
                {
                    isQuotedPart = false;
                    if (builder.Length > 0)
                    {
                        args.Add(builder.ToString());
                        builder.Clear();
                    }
                }
            }
            else
            {
                builder.Append(input[i]);
            }
        }

        // Adds last arg:
        if (builder.Length > 0)
            args.Add(builder.ToString());

        return args;
    }
}
