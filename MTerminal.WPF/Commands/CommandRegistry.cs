using System.Collections.Immutable;

namespace MTerminal.WPF.Commands;

/// <summary>
/// Holds and manages Terminal commands.
/// </summary>
public class CommandRegistry
{
    /// <summary>
    /// Gets a readonly collection of all registered commands.
    /// </summary>
    public IEnumerable<TerminalCommand> RegisteredCommands { get => _commands.ToArray(); }

    /// <summary>
    /// Gets a readonly collection of command names and their aliases.
    /// </summary>
    public IEnumerable<string> CommandNames { get
        {
            var names = new List<string>();
            foreach (var command in RegisteredCommands)
            {
                names.Add(command.Command);
                names.AddRange(command.Aliases);
            }
            return names.ToImmutableArray();
        }
    }

    private readonly List<TerminalCommand> _commands;

    internal CommandRegistry()
    {
        _commands = new List<TerminalCommand>();
    }

    /// <summary>
    /// Adds a command to the Commands list.<br></br>
    /// '<see cref="TerminalCommand.Command"/>' should be unique. Commands are case insensitive.
    /// </summary>
    /// <param name="command">Command to add.</param>
    /// <exception cref="ArgumentException">Thrown if command with that <see cref="TerminalCommand.Command"/> is already added.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <see cref="TerminalCommand.Command"/> is null.</exception>
    public void Add(TerminalCommand command)
    {
        if (Find(command.Command) is not null)
            throw new ArgumentException($"Command '{command.Command}' is already added. Commands are case insensitive.");

        _commands.Add(command);
    }

    /// <summary>
    /// Searches specified command in a list of registered commands and returns it if found.
    /// </summary>
    /// <param name="command">Command to find.</param>
    /// <returns>Found command or <see langword="null"/> if not found.</returns>
    public TerminalCommand? Find(string command)
    {
        foreach (var cmd in _commands)
        {
            if (cmd.Command.Equals(command, StringComparison.InvariantCultureIgnoreCase)
                || cmd.Aliases.Contains(command, StringComparer.InvariantCultureIgnoreCase))
            {
                return cmd;
            }
        }
        return null;
    }

    /// <summary>
    /// Removes a command from registered commands.
    /// </summary>
    /// <param name="command">Command to remove.</param>
    /// <returns><see langword="true"/> if successfully removed.</returns>
    public bool Remove(TerminalCommand command)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        return _commands.Remove(command);
    }

    /// <summary>
    /// Removes a command by name from registered commands.
    /// </summary>
    /// <param name="command">Command to remove.</param>
    /// <returns><see langword="true"/> if successfully removed.</returns>
    public bool Remove(string command)
    {
        return Find(command) is TerminalCommand cmd && Remove(cmd);
    }

    /// <summary>Indicates whether specified command is registered.</summary>
    public bool Contains(TerminalCommand command) => Find(command.Command) is not null;
    
    /// <summary>Indicates whether specified command name is registered.</summary>
    public bool Contains(string command) => Find(command) is not null;
}
