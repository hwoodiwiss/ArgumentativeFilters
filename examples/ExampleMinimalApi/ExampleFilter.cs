using System.Threading.Tasks;
using ArgumentativeFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ExampleMinimalApi;

public static partial class ExampleFilter
{
    [ArgumentativeFilter]
    private static ValueTask<object?> NormalizeRouteStringsFilterRef(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next,
        ref string country,
        ref int id,
        [IndexOf(nameof(country))] int countryIndex,
        [FromServices] IConfiguration? configuration)
    {
        if (id < 0)
        {
            return ValueTask.FromResult<object?>(Results.BadRequest("id must be positive"));
        }
        
        context.Arguments[countryIndex] = country.ToUpperInvariant();
        return next(context);
    }
}