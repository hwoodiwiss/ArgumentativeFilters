namespace ArgumentativeFilters.Tests;

[UsesVerify]
public class FilterFactorySnapshotTests
{
    [Fact]
    public Task GeneratesFilterExtensionsCorrectly()
    {
        // The source code to test
        var source = @"
using ArgumentativeFilters;

public static class ExampleFilter
{
    [ArgumentativeFilter]
    public static string NormalizeRouteStringsFilter(string context, string next, string country)
    {
        return next;
    }
}
";

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task GeneratesFilterFactoryCorrectly()
    {
        // The source code to test
        var source = @"
using ArgumentativeFilters;
using Microsoft.AspNetCore.Http;

public static class ExampleFilter
{
    [ArgumentativeFilter]
    public static string NormalizeRouteStringsFilter(string context, string next, string country)
    {
        return next(context);
    }
}
";

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task GeneratesFilterFactoryCorrectly2()
    {
        // The source code to test
        var source = @"
using ArgumentativeFilters;

namespace ExampleMinimalApi;

public class ExampleFilter2
{
    [ArgumentativeFilter]
    public static string NormalizeRouteStringsFilter(string context, string next, string country, int countryIndex)
    {
        return next(context);
    }
}
";

        return TestHelper.Verify(source);
    }
}