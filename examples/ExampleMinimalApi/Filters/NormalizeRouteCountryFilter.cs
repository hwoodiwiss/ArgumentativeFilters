using System.Threading.Tasks;

using ArgumentativeFilters;

using Microsoft.AspNetCore.Http;

namespace ExampleMinimalApi.Filters;

public static partial class NormalizeRouteCountryFilter
{
    [ArgumentativeFilter]
    private static ValueTask<object?> Filter(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next,
        ref string country,
        [IndexOf(nameof(country))] int countryIndex)
    {
        context.Arguments[countryIndex] = country.ToUpperInvariant();
        return next(context);
    }
}
