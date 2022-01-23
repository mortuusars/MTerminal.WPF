using MTerminal.WPF;
using MTerminal.WPF.Commands;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MTerminal.Tests
{
    public class CommandsTests
    {
        public CommandRegistry _sut = new CommandRegistry();

        [Fact]
        public void AddShouldAddCommandToList()
        {
            TerminalCommand command = new TerminalCommand("test", "test", (s) => { });
            _sut.Add(command);

            IEnumerable<TerminalCommand> list = _sut.GetCommands();

            Assert.True(_sut.Contains("test"));
        }

        [Fact]
        public void FindShouldFindCommandWithDifferentStringCase()
        {
            TerminalCommand command = new TerminalCommand("test", "test", (s) => { });
            _sut.Add(command);

            Assert.Equal(command, _sut.Find("tEsT"));
        }
    }
}