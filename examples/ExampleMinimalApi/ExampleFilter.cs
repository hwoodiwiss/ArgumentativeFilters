using System;
using System.Threading.Tasks;

using ArgumentativeFilters;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ExampleMinimalApi;

public static partial class ExampleFilter
{
    [ArgumentativeFilter]
    private static ValueTask<object?> NormalizeRouteStringsFilter(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next,
        string country,
        [IndexOf(nameof(country))] int countryIndex,
        [FromServices] IConfiguration? configuration)
    {
        context.Arguments[countryIndex] = country.ToUpperInvariant();
        return next(context);
    }
}