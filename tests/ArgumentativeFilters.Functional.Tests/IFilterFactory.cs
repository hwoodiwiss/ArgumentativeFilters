using Microsoft.AspNetCore.Http;

namespace ArgumentativeFilters.Functional.Tests;

internal interface IFilterFactory
{
    internal static abstract EndpointFilterDelegate Factory(EndpointFilterFactoryContext context, EndpointFilterDelegate nextFilter);
}