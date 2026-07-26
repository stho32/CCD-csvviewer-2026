using CsvViewer.BL.Common;
using CsvViewer.BL.Interaction.Data;
using CsvViewer.BL.Interaction.Operations;
using CsvViewer.BL.Interaction;

namespace CsvViewer.BL.Tests.Navigation;

public class NavigationCommandMapperTests
{
    [TestCase('F', NavigationCommand.First)]
    [TestCase('f', NavigationCommand.First)]
    [TestCase('P', NavigationCommand.Previous)]
    [TestCase('p', NavigationCommand.Previous)]
    [TestCase('N', NavigationCommand.Next)]
    [TestCase('n', NavigationCommand.Next)]
    [TestCase('L', NavigationCommand.Last)]
    [TestCase('l', NavigationCommand.Last)]
    [TestCase('E', NavigationCommand.Exit)]
    [TestCase('e', NavigationCommand.Exit)]
    [TestCase('?', NavigationCommand.None)]
    public void Wenn_TasteGemapptWird_dann_ErwarteterBefehlEntsteht(
        char key,
        NavigationCommand expected)
    {
        // Arrange
        char originalKey = key;

        // Act
        Result<NavigationCommand> result = NavigationCommandMapper.Map(key);

        // Assert
        Assert.That(result.IsSuccess, Is.True, result.Message);
        Assert.That(result.Value, Is.EqualTo(expected));
        Assert.That(key, Is.EqualTo(originalKey));
    }
}
