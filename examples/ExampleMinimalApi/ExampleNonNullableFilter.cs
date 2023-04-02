using System.Threading.Tasks;

using ArgumentativeFilters;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ExampleMinimalApi;

#nullable disable

internal static partial class ExampleNonNullableFilter
{
    [ArgumentativeFilter]
    private static ValueTask<object> NormalizeRouteStringsFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, string country, [IndexOf(nameof(country))] int countryIndex, [FromServices] IConfiguration configuration)
    {
        context.Arguments[countryIndex] = country.ToUpperInvariant();
        return next(context);
    }
}