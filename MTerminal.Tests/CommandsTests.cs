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
            TerminalCommand<string> command = new TerminalCommand<string>("test", "test", (s) => { });
            _sut.Add(command);

            IEnumerable<ITerminalCommand> list = _sut.GetCommands();

            Assert.Equal(command, list.First());
        }

        [Fact]
        public void FindShouldFindCommandWithDifferentStringCase()
        {
            TerminalCommand<string> command = new TerminalCommand<string>("test", "test", (s) => { });
            _sut.Add(command);

            Assert.Equal(command, _sut.Find("tEsT"));
        }
    }
}