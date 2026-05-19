using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ArgumentativeFilters.Functional.Tests.Behaviours;

public partial class MiddlewareServiceTests : ArgumentativeMiddlewareTests
{
    [Fact]
    public async Task ArgumentativeMiddleware_WhenRequiredServiceExists_ResolvesServiceAndInvokesNext()
    {
        // Arrange
        Services.AddScoped(_ => new MiddlewareTestService("resolved"));

        // Act
        await TestMiddlewareBehaviour<RequiredServiceMiddleware>(async (context, middleware, uniqueFallbackResult) =>
        {
            await middleware(context);

            // Assert
            context.Items[nameof(MiddlewareTestService)].ShouldBe("resolved");
            context.Items[nameof(uniqueFallbackResult)].ShouldBe(uniqueFallbackResult);
        });
    }

    [Fact]
    public async Task ArgumentativeMiddleware_WhenOptionalServiceIsMissing_PassesNullToMiddleware()
    {
        // Act
        await TestMiddlewareBehaviour<OptionalServiceMiddleware>(async (context, middleware, uniqueFallbackResult) =>
        {
            await middleware(context);

            // Assert
            context.Items[nameof(OptionalServiceMiddleware)].ShouldBe(true);
            context.Items[nameof(uniqueFallbackResult)].ShouldBe(uniqueFallbackResult);
        });
    }

    private sealed record MiddlewareTestService(string Value);

    private interface IMissingMiddlewareTestService;

    public partial class RequiredServiceMiddleware : IMiddlewareFactory
    {
        [ArgumentativeMiddleware]
        private static Task InvokeAsync(HttpContext context, RequestDelegate next, MiddlewareTestService service)
        {
            context.Items[nameof(MiddlewareTestService)] = service.Value;
            return next(context);
        }
    }

    public partial class OptionalServiceMiddleware : IMiddlewareFactory
    {
        [ArgumentativeMiddleware]
        private static Task InvokeAsync(HttpContext context, RequestDelegate next, IMissingMiddlewareTestService? service)
        {
            context.Items[nameof(OptionalServiceMiddleware)] = service is null;
            return next(context);
        }
    }
}
