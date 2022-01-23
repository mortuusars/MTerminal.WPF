using MTerminal.WPF.Autocomplete;
using System.Windows.Media;

namespace MTerminal.WPF.Commands;

public class CommandRegistry
{
    internal Dictionary<string, TerminalCommand> RegisteredCommands { get; }
    internal List<TerminalCommand> CommandsList { get; }

    internal CommandRegistry()
    {
        RegisteredCommands = new Dictionary<string, TerminalCommand>();
        CommandsList = new List<TerminalCommand>();

        Add(new HelpTerminalCommand(CommandsList));
        Add(new TerminalCommand("clear", aliases: new[] { "cls" }, "Clears the Terminal screen.", (_) => Terminal.Clear()));
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

        RegisteredCommands.Add(command.Command, command);

        if (command.Aliases.Count > 0)
        {
            foreach (var alias in command.Aliases)
            {
                RegisteredCommands.Add(alias, command);
            }
        }

        CommandsList.Add(command);
    }

    public TerminalCommand? Find(string command)
    {
        return RegisteredCommands.FirstOrDefault(c => c.Key.Equals(command, StringComparison.InvariantCultureIgnoreCase)).Value;
    }
    public bool Remove(TerminalCommand command) => 
        RegisteredCommands.Remove(command.Command) & CommandsList.Remove(command); // Singular & operator to execute both parts.
    public bool Remove(string command) => Find(command) is TerminalCommand cmd && Remove(cmd);
    public bool Contains(TerminalCommand command) => Find(command.Command) is not null;
    public bool Contains(string command) => Find(command) is not null;
    public IEnumerable<TerminalCommand> GetCommands() => CommandsList.ToArray();
}
