namespace MTerminal.WPF.Autocomplete
{
    internal class CommandAutocomplete
    {
        /// <summary>
        /// Gets the first command that matches input string, or starts with input string.
        /// </summary>
        /// <param name="commands">Collection of commands</param>
        /// <returns>Matched command or empty command if nothing matched.</returns>
        internal static TerminalCommand Match(string input, IEnumerable<TerminalCommand> commands)
        {
            if (string.IsNullOrWhiteSpace(input) || commands is null || commands.Count() == 0)
                return TerminalCommand.Empty;

            string startingChars = input.Trim();
            return commands.FirstOrDefault(c => c.Command.StartsWith(startingChars), TerminalCommand.Empty);
        }

        /// <summary>
        /// Gets the first alphabetically ordered command that matches input string, or starts with input string.
        /// </summary>
        /// <param name="commands">Collection of commands</param>
        /// <returns>Matched command or empty command if nothing matched.</returns>
        internal static TerminalCommand MatchOrdered(string input, IEnumerable<TerminalCommand> commands)
        {
            if (string.IsNullOrWhiteSpace(input) || commands is null || commands.Count() == 0)
                return TerminalCommand.Empty;

            return commands.OrderBy(c => c.Command).FirstOrDefault(c => c.Command.StartsWith(input), TerminalCommand.Empty);
        }

        /// <summary>
        /// Gets the next alphabetically ordered command that matches input string, or starts with input string.
        /// </summary>
        /// <param name="commands">Collection of commands</param>
        /// <returns>Matched command or empty command if nothing matched.</returns>
        internal static TerminalCommand GetNextOrdered(string input, IEnumerable<TerminalCommand> commands)
        {
            if (string.IsNullOrWhiteSpace(input) || commands is null || commands.Count() == 0)
                return TerminalCommand.Empty;

            var ordered = commands.OrderBy(c => c.Command);

            foreach (var cmd in ordered)
            {
                if (string.Compare(cmd.Command, input) > 0)
                    return cmd;
            }

            return ordered.FirstOrDefault(TerminalCommand.Empty);
        }

        /// <summary>
        /// Matches all commands that start with input string.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="commands">Collection of commands</param>
        /// <returns>Collection of matched commands or empty collection if none matched.</returns>
        internal static IEnumerable<TerminalCommand> MatchAll(string input, IEnumerable<TerminalCommand> commands)
        {
            if (string.IsNullOrWhiteSpace(input) || commands is null || commands.Count() == 0)
                return Array.Empty<TerminalCommand>();

            var inputTrimmed = input.Trim();
            return commands.OrderBy(c => c.Command).Where(c => c.Command.StartsWith(inputTrimmed));
        }
    }
}
