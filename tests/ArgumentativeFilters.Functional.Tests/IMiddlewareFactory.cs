using Microsoft.AspNetCore.Http;

namespace ArgumentativeFilters.Functional.Tests;

internal interface IMiddlewareFactory
{
    internal static abstract RequestDelegate Middleware(RequestDelegate next);
}
