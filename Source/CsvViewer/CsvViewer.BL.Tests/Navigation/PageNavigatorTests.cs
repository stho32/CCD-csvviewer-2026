using CsvViewer.BL.Common;
using CsvViewer.BL.Navigation;

namespace CsvViewer.BL.Tests.Navigation;

public class PageNavigatorTests
{
    [Test]
    public void Wenn_VorwaertsUndRueckwaertsNavigiertWird_dann_IndexAendertSichJeSchritt()
    {
        // Arrange
        PageNavigator navigator = CreateNavigator(3);
        int initialIndex = navigator.CurrentPageIndex;

        // Act
        Result<int> firstNext = navigator.Apply(NavigationCommand.Next);
        int afterFirstNext = navigator.CurrentPageIndex;
        Result<int> secondNext = navigator.Apply(NavigationCommand.Next);
        int afterSecondNext = navigator.CurrentPageIndex;
        Result<int> previous = navigator.Apply(NavigationCommand.Previous);

        // Assert
        Assert.That(initialIndex, Is.Zero);
        Assert.That(firstNext.IsSuccess, Is.True);
        Assert.That(afterFirstNext, Is.EqualTo(1));
        Assert.That(secondNext.IsSuccess, Is.True);
        Assert.That(afterSecondNext, Is.EqualTo(2));
        Assert.That(previous.IsSuccess, Is.True);
        Assert.That(navigator.CurrentPageIndex, Is.EqualTo(1));
    }

    [Test]
    public void Wenn_FirstUndLastAngewendetWerden_dann_RandseitenWerdenErreicht()
    {
        // Arrange
        PageNavigator navigator = CreateNavigator(4);
        navigator.Apply(NavigationCommand.Next);
        int beforeLast = navigator.CurrentPageIndex;

        // Act
        Result<int> last = navigator.Apply(NavigationCommand.Last);
        int afterLast = navigator.CurrentPageIndex;
        Result<int> first = navigator.Apply(NavigationCommand.First);

        // Assert
        Assert.That(beforeLast, Is.EqualTo(1));
        Assert.That(last.IsSuccess, Is.True);
        Assert.That(afterLast, Is.EqualTo(3));
        Assert.That(first.IsSuccess, Is.True);
        Assert.That(navigator.CurrentPageIndex, Is.Zero);
    }

    [Test]
    public void Wenn_UeberBeideRaenderNavigiertWird_dann_IndexBleibtGeklemmt()
    {
        // Arrange
        PageNavigator navigator = CreateNavigator(2);
        int beforePrevious = navigator.CurrentPageIndex;

        // Act
        navigator.Apply(NavigationCommand.Previous);
        int afterPrevious = navigator.CurrentPageIndex;
        navigator.Apply(NavigationCommand.Last);
        int beforeNext = navigator.CurrentPageIndex;
        navigator.Apply(NavigationCommand.Next);

        // Assert
        Assert.That(afterPrevious, Is.EqualTo(beforePrevious));
        Assert.That(navigator.CurrentPageIndex, Is.EqualTo(beforeNext));
    }

    [TestCase(NavigationCommand.None)]
    [TestCase(NavigationCommand.Exit)]
    public void Wenn_BefehlKeineNavigationAusloest_dann_IndexBleibtUnveraendert(
        NavigationCommand command)
    {
        // Arrange
        PageNavigator navigator = CreateNavigator(3);
        navigator.Apply(NavigationCommand.Next);
        int before = navigator.CurrentPageIndex;

        // Act
        Result<int> result = navigator.Apply(command);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(navigator.CurrentPageIndex, Is.EqualTo(before));
    }

    [Test]
    public void Wenn_UnbekannterBefehlAngewendetWird_dann_IndexBleibtUnveraendert()
    {
        // Arrange
        PageNavigator navigator = CreateNavigator(2);
        int before = navigator.CurrentPageIndex;

        // Act
        Result<int> result = navigator.Apply((NavigationCommand)999);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Value, Is.EqualTo(before));
        Assert.That(navigator.CurrentPageIndex, Is.EqualTo(before));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Wenn_SeitenzahlNichtPositivIst_dann_KeinNavigatorEntsteht(int pageCount)
    {
        // Arrange & Act
        Result<PageNavigator> result = PageNavigator.Create(pageCount);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Value, Is.Null);
    }

    private static PageNavigator CreateNavigator(int pageCount)
    {
        Result<PageNavigator> result = PageNavigator.Create(pageCount);
        Assert.That(result.IsSuccess, Is.True, result.Message);
        return result.Value!;
    }
}
