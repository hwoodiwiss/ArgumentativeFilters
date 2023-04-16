using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ArgumentativeFilters.Integration.Tests;

public abstract class ArgumentativeFilterTests : IDisposable
{
    private IServiceScope? _serviceScope;
    private ArgumentativeFilterTestContext? _context;
    
    /// <summary>
    /// Services to be made available via the scoped ServiceProvider when creating the test context.
    /// </summary>
    public ServiceCollection Services { get; } = new ();
    
    /// <summary>
    /// The test context create from initializing the test.
    /// </summary>
    public ArgumentativeFilterTestContext Context 
    {
        get => _context 
             ?? throw new InvalidOperationException("Attempted to use uninitialized test context. Call Initialize on the test before attempting to get the Context"); 
    }

    [MemberNotNull(nameof(_context))]
    protected void SetupContext(Delegate endpointDelegate, IList<object?> argumentValues)
    {
        ArgumentNullException.ThrowIfNull(endpointDelegate);
        ServiceProvider serviceProvider = Services.BuildServiceProvider();
        _serviceScope = serviceProvider.CreateScope();
        
        EndpointFilterFactoryContext factoryContext = new ()
        {
            ApplicationServices = serviceProvider,
            MethodInfo = endpointDelegate.Method
        };

        HttpContext httpContext = new DefaultHttpContext { RequestServices = _serviceScope.ServiceProvider };
        DefaultEndpointFilterInvocationContext invocationContext = new (
            httpContext,
            argumentValues);

        _context = new()
        {
            FactoryContext = factoryContext,
            InvocationContext = invocationContext,
        };
    }
    
    protected static EndpointFilterDelegate NullEndpointFilterDelegate { get; } = _ => ValueTask.FromResult<object?>(null);
    
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
}