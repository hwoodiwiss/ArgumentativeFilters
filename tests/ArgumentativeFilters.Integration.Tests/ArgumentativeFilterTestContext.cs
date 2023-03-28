using Microsoft.AspNetCore.Http;

namespace ArgumentativeFilters.Integration.Tests;

public class ArgumentativeFilterTestContext
{
    public required EndpointFilterFactoryContext FactoryContext { get; init; }
    
    public required EndpointFilterInvocationContext InvocationContext { get; init; }
}