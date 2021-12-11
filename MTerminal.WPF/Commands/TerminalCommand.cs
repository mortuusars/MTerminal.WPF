using System.Windows.Input;

namespace MTerminal.WPF;

public class TerminalCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// "Id" of this command. This is what you type to find and invoke a command.
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
    /// Optional default parameter that will be passet to Execute method when command executes.
    /// </summary>
    public string DefaultParameter { get; set; }
    /// <summary>
    /// Action with a parameter of a specified type, that will be executed.
    /// </summary>
    public Action<string> Action { get; set; }

    public static readonly TerminalCommand Empty = new TerminalCommand(string.Empty);

    /// <summary>
    /// Creates an instance of a Terminal Command and sets provided properties.
    /// </summary>
    /// <param name="command">Command "id". This is what user types to invoke it.</param>
    /// <param name="description">Short desction of a command.</param>
    /// <param name="action">Action that will be executed when this command is invoked.</param>
    public TerminalCommand(string command, string description, Action<string> action)
    {
        Command = command;
        Description = description;
        Action = action;

        DefaultParameter = string.Empty;
        DetailedDescription = string.Empty;
    }

    /// <summary>
    /// Creates an instance of a Terminal Command with a name and leaves other properties empty.
    /// </summary>
    /// <param name="command"></param>
    public TerminalCommand(string command) : this(command, string.Empty, (_) => { }) { }

    /// <summary>
    /// Checks if the command is empty.<br/>
    /// Command is considered empty if <see cref="Command"/> is null or empty or has only whitespace.
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(Command);
    }

    /// <summary>
    /// This command can always execute.
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public bool CanExecute(object? parameter) => true;

    /// <summary>
    /// Executes command with given parameter. Parameter should be as string.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when parameter is not string.</exception>
    public void Execute(object? parameter)
    {
        if (parameter is string param)
            Action(param);
        else
            throw new ArgumentException("Parameter type must be string.", nameof(parameter));
    }

    /// <summary>
    /// Executes command with given string parameter.
    /// </summary>
    public void Execute(string parameter)
    {
        Action(parameter);
    }

    public override string ToString()
    {
        return Command;
    }
}