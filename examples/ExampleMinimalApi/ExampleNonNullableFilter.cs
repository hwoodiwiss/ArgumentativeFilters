using ArgumentativeFilters;
using Microsoft.AspNetCore.Mvc;

namespace ExampleMinimalApi;

#nullable disable

public static partial class ExampleNonNullableFilter
{
    [ArgumentativeFilter]
    public static ValueTask<object> NormalizeRouteStringsFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, string country,  int countryIndex, [FromServices] IConfiguration configuration)
    {
        context.Arguments[countryIndex] = country.ToUpperInvariant();
        return next(context);
    }
}