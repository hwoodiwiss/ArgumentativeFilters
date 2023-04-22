using Microsoft.AspNetCore.Http;

namespace ArgumentativeFilters.Integration.Tests;

internal interface IFilterFactory
{
    internal static abstract EndpointFilterDelegate Factory(EndpointFilterFactoryContext context, EndpointFilterDelegate nextFilter);
}