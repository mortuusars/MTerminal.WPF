using MTerminal.WPF.Utils;
using Xunit;

namespace MTerminal.Tests;

public class HistoryTests
{
    [Fact]
    public void ShouldReturnLastItemIfPreviousCalledOnce()
    {
        var history = new History<string>(3);
        history.Append("1"); // Deleted
        history.Append("2");
        history.Append("3");
        history.Append("4");
        
        var previous = history.GetPrevious();

        Assert.Equal("4", previous);
    }

    [Fact]
    public void ShouldReturnPreviousItems()
    {
        var ch = new History<string>(3);
        ch.Append("1"); // Deleted
        ch.Append("2");
        ch.Append("3");
        ch.Append("4");

        ch.GetPrevious();
        ch.GetPrevious();
        var thirdPrevious = ch.GetPrevious();

        Assert.Equal("2", thirdPrevious);
    }

    [Fact]
    public void ShouldReturnNextItems()
    {
        var history = new History<string>(3);
        history.Append("1"); // Deleted
        history.Append("2");
        history.Append("3");
        history.Append("4");

        var firstNext = history.GetNext();
        Assert.Equal("2", firstNext); // 2 is first becase capacity is only 3 items - so first item is already deleted.

        history.GetNext();
        var thirdNext = history.GetNext();
        Assert.Equal("4", thirdNext);
    }

    [Fact]
    public void ShouldTraverseBackAndForthNicely()
    {
        var history = new History<string>(4);
        history.Append("1"); // Deleted because capacity is 4 items.
        history.Append("2"); // Deleted
        history.Append("3");
        history.Append("4");
        history.Append("5");
        history.Append("6");

        history.GetNext();
        var secondNext = history.GetNext();
        Assert.Equal("4", secondNext);

        history.GetPrevious();
        history.GetPrevious();
        var thirdPrevious = history.GetPrevious();
        Assert.Equal("5", thirdPrevious);
    }

    [Fact]
    public void ShouldReturnProperTraversingIndex()
    {
        var history = new History<string>(3);
        history.Append("1"); // Deleted
        history.Append("2");
        history.Append("3");
        history.Append("4");

        history.GetNext();
        history.GetNext();

        // index in a list:
        Assert.Equal(1, history.TraversingIndex);
    }

    [Fact]
    public void ShouldReturnProperIfNoOverflow()
    {
        var history = new History<string>(3);
        history.CycleGettters = false;
        history.Append("1"); // Deleted
        history.Append("2");
        history.Append("3");
        history.Append("4");

        history.GetNext();
        history.GetNext();
        history.GetNext();
        history.GetNext();

        Assert.Equal("4", history.GetNext());

        history.GetPrevious();
        history.GetPrevious();
        history.GetPrevious();
        history.GetPrevious();
        history.GetPrevious();
        
        Assert.Equal("2", history.GetPrevious());
    }

    [Fact]
    public void ShouldNotThrowIfNoItems()
    {
        var history = new History<string>(3);

        Assert.Equal(0, history.Count);
        Assert.Equal(0, history.TraversingIndex);
    }
}
