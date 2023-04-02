using Microsoft.AspNetCore.Http;

namespace ArgumentativeFilters.Integration.Tests.Behaviours;

public partial class NamespaceTests : ArgumentativeFilterTests
{
    public NamespaceTests()
    {
        Initialize((string test) => test, new List<object?> { "testValue" });
    }

    // internal static partial class TestFilterParent
    // {
    //     [ArgumentativeFilter]
    //     internal static async ValueTask<object?> TestFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, string test)
    //     {
    //         return await next(context);
    //     }
    // }
}