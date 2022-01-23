namespace MTerminal.WPF;

public class TerminalCommand
{
    /// <summary>
    /// "Id" of this command.
    /// </summary>
    public string Command { get; set; }

    /// <summary>
    /// Description of a command.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Full description of a command with all the details.
    /// </summary>
    public string DetailedDescription { get; set; }

    /// <summary>
    /// Action with a parameter of a specified type, that will be executed.
    /// </summary>
    public Action<IEnumerable<string>> Action { get; set; }

    public List<string> Aliases { get; set; } = new List<string>();

    #region Constructors

    /// <summary>
    /// Indicates whether the command should throw an exception if executing <see cref="Action"/> is failed.<br></br>
    /// Otherwise exception and its stack trace will be printed to the Terminal.<br></br><br></br>
    /// Default is <see langword="false"/>.
    /// </summary>
    public bool ThrowsException { get; set; } = false;

    public static readonly TerminalCommand Empty = new TerminalCommand();

    /// <summary>
    /// Creates an instance of a Terminal Command and sets provided properties.
    /// </summary>
    /// <param name="command">Command "id". This is what user types to invoke it.</param>
    /// <param name="description">Short desction of a command.</param>
    /// <param name="action">Action that will be executed when this command is invoked.</param>
    public TerminalCommand(string command, string[] aliases, string description, Action<IEnumerable<string>> action)
    {
        Command = command;
        Aliases = new List<string>(aliases);
        Description = description;
        Action = action;

        DetailedDescription = string.Empty;
    }

    /// <summary>
    /// Creates an instance of a Terminal Command and sets provided properties.
    /// </summary>
    /// <param name="command">Command "id". This is what user types to invoke it.</param>
    /// <param name="description">Short desction of a command.</param>
    /// <param name="action">Action that will be executed when this command is invoked.</param>
    public TerminalCommand(string command, string description, Action<IEnumerable<string>> action)
    {
        Command = command;
        Description = description;
        Action = action;
        
        DetailedDescription = string.Empty;
    }

    /// <summary>
    /// Creates an instance of a Terminal Command and sets provided properties.
    /// </summary>
    /// <param name="command">Command "id". This is what user types to invoke it.</param>
    /// <param name="action">Action that will be executed when this command is invoked.</param>
    public TerminalCommand(string command, Action<IEnumerable<string>> action)
    {
        Command = command;
        Action = action;
        
        Description = string.Empty;
        DetailedDescription = string.Empty;
    }

    /// <summary>
    /// Creates an instance of a Terminal Command and sets provided properties.
    /// </summary>
    /// <param name="command">Command "id". This is what user types to invoke it.</param>
    public TerminalCommand(string command)
    {
        Command = command;

        Action = (_) => { };
        Description = string.Empty;
        DetailedDescription = string.Empty;
    }

    /// <summary>
    /// Creates an instance of a Terminal Command that is equivalent to a <see cref="Empty"/>.
    /// </summary>
    public TerminalCommand()
    {
        Command = string.Empty;
        Description = string.Empty;
        Action = (_) => { };
        DetailedDescription = string.Empty;
    }

    #endregion

    /// <summary>
    /// Adds an alias to command.
    /// </summary>
    /// <param name="alias">Alias that will be added.</param>
    /// <returns>Same command instance to chain calls.</returns>
    public TerminalCommand AddAlias(string alias)
    {
        Aliases.Add(alias);
        return this;
    }

    /// <summary>
    /// Executes command with given arguments.
    /// </summary>
    public void Execute(IEnumerable<string> args)
    {
        Action(args);
    }

    /// <summary>
    /// Checks if the command is empty.<br/>
    /// Command is considered empty if <see cref="Command"/> is null or empty or has only whitespace.
    /// </summary>
    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(Command);
    }

    /// <summary>
    /// Returns a string from <see cref="Command"/> its aliases and <see cref="Description"/>.
    /// </summary>
    public override string ToString()
    {
        if (Aliases.Count == 0)
            return Command;

        return $"{Command} / {string.Join(" / ", Aliases)}";
    }
}