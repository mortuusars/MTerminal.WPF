using MTerminal.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Xunit;

namespace MTerminal.Tests;

public class ColorExtensionsTests
{
    [Theory]
    [InlineData(0.0d, 0x00)]
    [InlineData(-2.0d, 0x00)]
    [InlineData(1.0d, 0xff)]
    [InlineData(4.0d, 0xff)]
    [InlineData(0.5d, 0x7f)]
    public void MultiplierShouldWorkCorrectly(double multiplier, byte expected)
    {
        var color = Colors.Black.WithOpacity(multiplier);
        Assert.Equal(expected, color.A);
    }
}
