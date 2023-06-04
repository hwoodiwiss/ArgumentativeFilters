using Microsoft.AspNetCore.Http;

namespace ArgumentativeFilters.Functional.Tests.Behaviours;

public partial class ValueNullabilityTests : ArgumentativeFilterTests
{
    [Fact]
    public async Task NonNullableParameter_WhenMissingDuringCreation_CausesFilterFallback()
    {
        await TestFilterFactoryBehaviour<NonNullableParameterFilter>(
            (string notTest) => notTest,
            async (context, filter, fallbackResult) => {
                // Act
                var result = await filter(context.InvocationContext);
        
                // Assert
                result.ShouldBe(fallbackResult);
            });
    }
    
    [Fact]
    public async Task NullableParameter_WhenMissingDuringCreation_RunsFilter()
    {
        await TestFilterFactoryBehaviour<NullableParameterFilter>(
            (string notTest) => notTest,
            async (context, filter, _) => {
                // Act
                var result = await filter(context.InvocationContext);
        
                // Assert
                result.ShouldBeNull();
            });
    }
    
    public partial class NonNullableParameterFilter : IFilterFactory
    {
        [ArgumentativeFilter]
        public static ValueTask<object?> TestFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, int test)
        {
            return ValueTask.FromResult<object?>(null);
        }
    }
    
    public partial class NullableParameterFilter : IFilterFactory
    {
        [ArgumentativeFilter]
        public static ValueTask<object?> TestFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, int? test)
        {
            return ValueTask.FromResult<object?>(null);
        }
    }
    
}