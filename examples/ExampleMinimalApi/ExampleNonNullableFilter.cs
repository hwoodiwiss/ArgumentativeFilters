using System.Threading.Tasks;

using ArgumentativeFilters;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ExampleMinimalApi;

#nullable disable

public sealed partial class ParentClass
{
    internal static partial class ExampleNonNullableFilter
    {
        [ArgumentativeFilter]
        private static ValueTask<object> NormalizeRouteStringsFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, string country, [IndexOf(nameof(country))] int countryIndex, [FromServices] IConfiguration configuration)
        {
            context.Arguments[countryIndex] = country.ToUpperInvariant();
            return next(context);
        }
    }
}
internal static partial class ExampleNonNullableFilter
{
    [ArgumentativeFilter]
    private static ValueTask<object> NormalizeRouteStringsFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, string country, [IndexOf(nameof(country))] int countryIndex, [FromServices] IConfiguration configuration)
    {
        context.Arguments[countryIndex] = country.ToUpperInvariant();
        return next(context);
    }
}