using MTerminal.WPF;
using MTerminal.WPF.Commands;
using System.Linq;
using Xunit;

namespace MTerminal.Tests
{
    public class CommandRegistryTests
    {
        public CommandRegistry _sut = new CommandRegistry();

        [Fact]
        public void AddShouldAddCommandToList()
        {
            TerminalCommand command = new("test", "test", (_) => { });
            _sut.Add(command);

            Assert.Contains(command, _sut.RegisteredCommands);
        }

        [Fact]
        public void CommandNamesShouldHaveAllCommandsAndAliases()
        {
            TerminalCommand command = new("test", "test", (_) => { });
            command.AddAlias("testAlias");
            TerminalCommand command2 = new("asd", "asd", (_) => { });
            command.AddAlias("asdAlias");

            _sut.Add(command);
            _sut.Add(command2);

            var names = _sut.CommandNames;

            Assert.Equal(4, names.Count());
            Assert.Contains("test", names);
            Assert.Contains("testAlias", names);
            Assert.Contains("asdAlias", names);
            Assert.Contains("asdAlias", names);
        }

        [Fact]
        public void ContainsShouldReturnTrueIfContains()
        {
            TerminalCommand command = new("test", "test", (_) => { });
            _sut.Add(command);

            Assert.True(_sut.Contains("tEsT"));
            Assert.True(_sut.Contains(command));
        }

        [Fact]
        public void FindShouldFindCommandWithDifferentStringCase()
        {
            TerminalCommand command = new("test", "test", (_) => { });
            _sut.Add(command);

            Assert.Equal(command, _sut.Find("tEsT"));
        }

        [Fact]
        public void FindShouldFindCommandFromAlias()
        {
            TerminalCommand command = new TerminalCommand("test", "test", (_) => { }).AddAlias("testAlias");
            _sut.Add(command);

            Assert.Equal(command, _sut.Find("testAlias"));
        }

        [Fact]
        public void FindShouldReturnNullIfNotFound()
        {
            TerminalCommand command = new("test", "test", (_) => { });
            _sut.Add(command);

            Assert.Null(_sut.Find("missingCommand"));
        }

        [Fact]
        public void RemoveShouldRemoveTheCommand()
        {
            TerminalCommand command = new("test", "test", (_) => { });
            _sut.Add(command);
            _sut.Remove(command);

            Assert.DoesNotContain(command, _sut.RegisteredCommands);
        }
    }
}