using MTerminal.WPF;
using MTerminal.WPF.Commands;
using System.Collections.Generic;
using Xunit;

namespace MTerminal.Tests;

public class CommandInputProcessorTests
{
    internal CommandInputProcessor _sut;

    public CommandInputProcessorTests()
    {
        var commands = new CommandRegistry();
        commands.Add(new TerminalCommand("test"));
        commands.Add(new TerminalCommand("command"));

        _sut = new CommandInputProcessor(commands);
    }

    [Theory]
    [InlineData("command", "command", new string[0])]
    [InlineData("cOmMaNd", "command", new string[0])]
    [InlineData("   command   ", "command", new string[0])]
    [InlineData("   command asd  ", "command", new string[] { "asd" })]
    [InlineData("   command asd   123  ", "command", new string[] { "asd", "123" })]
    [InlineData("   command \"asd   123\"  ", "command", new string[] { "asd   123" })]
    [InlineData("   command \"asd   123\" asd ", "command", new string[] { "asd   123", "asd" })]
    [InlineData("   command \'asd   123\' asd ", "command", new string[] { "asd   123", "asd" })]
    [InlineData("\"asd   123\" asd ", "", new string[0])]
    public void ProcessTheory(string input, string expectedCommand, IEnumerable<string> expectedArgs)
    {
        var (command, args) = _sut.Process(input);

        Assert.Equal(expectedCommand, command.Command);
        Assert.Equal(expectedArgs, args);
    }
}
