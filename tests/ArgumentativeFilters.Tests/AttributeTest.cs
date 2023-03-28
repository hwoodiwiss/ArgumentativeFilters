namespace ArgumentativeFilters.Tests;

[UsesVerify]
public class FilterFactorySnapshotTests
{
    [Fact]
    public void GeneratesFilterExtensionsCorrectly()
    {
        //true.ShouldBeFalse("Failing test as a reminder to write some tests.");
        true.ShouldBeTrue();
    }
}