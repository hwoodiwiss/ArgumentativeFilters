using Microsoft.AspNetCore.Http;

namespace ArgumentativeFilters.Integration.Tests.Behaviours;

public partial class ValueNullabilityTests : ArgumentativeFilterTests
{
    [Fact]
    public async Task NonNullableParameter_WhenMissingDuringCreation_CausesFilterFallback()
    {
        // Arrange
        var uniqueResult = Guid.NewGuid();
        EndpointFilterDelegate uniqueDelegate = _ => ValueTask.FromResult<object?>(uniqueResult);
        SetupContext((int notTest) => notTest, new List<object?>());
        var filter = NonNullableParameterFilter.Factory(Context.FactoryContext, uniqueDelegate);
        
        // Act
        var result = await filter(Context.InvocationContext);
        
        // Assert
        result.ShouldBe(uniqueResult);
    }
    
    [Fact]
    public async Task NullableParameter_WhenMissingDuringCreation_RunsFilter()
    {
        // Arrange
        var uniqueResult = Guid.NewGuid();
        EndpointFilterDelegate uniqueDelegate = _ => ValueTask.FromResult<object?>(uniqueResult);
        SetupContext((int notTest) => notTest, new List<object?>());
        var filter = NullableParameterFilter.Factory(Context.FactoryContext, uniqueDelegate);
        
        // Act
        var result = await filter(Context.InvocationContext);
        
        // Assert
        result.ShouldBeNull();
    }
    
    internal static partial class NonNullableParameterFilter
    {
        [ArgumentativeFilter]
        public static ValueTask<object?> TestFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, int test)
        {
            return ValueTask.FromResult<object?>(null);
        }
    }
    
    internal static partial class NullableParameterFilter
    {
        [ArgumentativeFilter]
        public static ValueTask<object?> TestFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, int? test)
        {
            return ValueTask.FromResult<object?>(null);
        }
    }
    
}