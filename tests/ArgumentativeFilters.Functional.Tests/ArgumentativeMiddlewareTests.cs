using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ArgumentativeFilters.Functional.Tests;

public delegate Task ArgumentativeMiddlewareTest(HttpContext context, RequestDelegate middleware, Guid uniqueFallbackResult);

public abstract class ArgumentativeMiddlewareTests : IDisposable
{
    private IServiceScope? _serviceScope;
    private HttpContext? _context;

    /// <summary>
    /// Services to be made available via the scoped service provider when creating the test context.
    /// </summary>
    public ServiceCollection Services { get; } = [];

    /// <summary>
    /// The http context created from initializing the test.
    /// </summary>
    public HttpContext Context
    {
        get => _context
               ?? throw new InvalidOperationException("Attempted to use uninitialized test context. Call SetupContext on the test before attempting to get the Context");
    }

    [MemberNotNull(nameof(_context))]
    protected void SetupContext()
    {
        ServiceProvider serviceProvider = Services.BuildServiceProvider();
        _serviceScope = serviceProvider.CreateScope();

        _context = new DefaultHttpContext
        {
            RequestServices = _serviceScope.ServiceProvider
        };
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _serviceScope?.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    internal async Task TestMiddlewareBehaviour<T>(ArgumentativeMiddlewareTest test)
        where T : IMiddlewareFactory
    {
        ArgumentNullException.ThrowIfNull(test);
        var uniqueResult = Guid.NewGuid();
        SetupContext();
        RequestDelegate uniqueDelegate = context =>
        {
            context.Items["uniqueFallbackResult"] = uniqueResult;
            return Task.CompletedTask;
        };

        var middleware = T.Middleware(uniqueDelegate);
        await test(Context, middleware, uniqueResult);
    }
}
