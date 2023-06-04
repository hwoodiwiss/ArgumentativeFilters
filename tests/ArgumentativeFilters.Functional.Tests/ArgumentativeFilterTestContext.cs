using Microsoft.AspNetCore.Http;

namespace ArgumentativeFilters.Functional.Tests;

public class ArgumentativeFilterTestContext
{
    public required EndpointFilterFactoryContext FactoryContext { get; init; }
    
    public required EndpointFilterInvocationContext InvocationContext { get; init; }
}