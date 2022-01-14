using MTerminal.WPF;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MTerminal.Tests
{
    public class CommandsTests
    {
        public Commands _sut = new Commands();

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