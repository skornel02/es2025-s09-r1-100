using Backend.Extensions;

namespace Test.Extensions;

public class ContainerImportExtensionTests
{
    [Fact]
    public void TestContainerParsing()
    {
        var importLine = new List<string>
        {
            "WKRE1092594",
            "4",
            "4",
            "4",
            "1",
            "1704412800000",
        };

        var import = new List<List<string>> { new(), importLine };

        var imported = import.ParseContainers();
        Assert.NotNull(imported);

        var importedContainer = imported[0];

        Assert.Equal("WKRE1092594", importedContainer.Id);
        Assert.Equal(4, importedContainer.BlockId);
        Assert.Equal(4, importedContainer.BayNum);
        Assert.Equal(4, importedContainer.StackNum);
        Assert.Equal(1, importedContainer.TierNum);
        Assert.Equal(new DateTime(2024, 1, 5, 0, 0, 0, DateTimeKind.Utc), importedContainer.ArrivedAt);
    }
}
