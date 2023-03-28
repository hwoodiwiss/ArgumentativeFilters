using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ArgumentativeFilters.Integration.Tests;

public sealed class ArgumentativeFilterFixture : IDisposable
{
    private IServiceScope? _serviceScope;
    
    /// <summary>
    /// Services to be made available via the scoped ServiceProvider when creating the test context.
    /// </summary>
    public ServiceCollection Services { get; } = new ();

    public ArgumentativeFilterTestContext CreateContext(Delegate endpointSignature, IList<object?> arguments)
    {
        ArgumentNullException.ThrowIfNull(endpointSignature);
        ServiceProvider serviceProvider = Services.BuildServiceProvider();
        _serviceScope = serviceProvider.CreateScope();
        
        EndpointFilterFactoryContext factoryContext = new ()
        {
            ApplicationServices = serviceProvider,
            MethodInfo = endpointSignature.Method
        };

        HttpContext httpContext = new DefaultHttpContext {RequestServices = _serviceScope.ServiceProvider};
        DefaultEndpointFilterInvocationContext invocationContext = new(
            httpContext,
            arguments);

        return new() {FactoryContext = factoryContext, InvocationContext = invocationContext,};
    }

    public void Dispose()
    {
        _serviceScope?.Dispose();
    }
}