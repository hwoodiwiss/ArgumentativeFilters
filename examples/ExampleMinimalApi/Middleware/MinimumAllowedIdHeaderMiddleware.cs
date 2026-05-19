using System.Globalization;
using System.Threading.Tasks;

using ArgumentativeFilters;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ExampleMinimalApi.Middleware;

internal static partial class MinimumAllowedIdHeaderMiddleware
{
    internal const string HeaderName = "X-Minimum-Allowed-Id";

    [ArgumentativeMiddleware(Prefix = nameof(AddMinimumAllowedIdHeader))]
    private static Task AddMinimumAllowedIdHeader(HttpContext context, RequestDelegate next, IOptionsSnapshot<ExampleMinimalApiOptions> options)
    {
        context.Response.Headers[HeaderName] = options.Value.MinimumAllowedId.ToString(CultureInfo.InvariantCulture);
        return next(context);
    }
}
