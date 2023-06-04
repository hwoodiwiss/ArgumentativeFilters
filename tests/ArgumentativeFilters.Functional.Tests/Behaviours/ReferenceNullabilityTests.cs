using Microsoft.AspNetCore.Http;

namespace ArgumentativeFilters.Functional.Tests.Behaviours;

public partial class ReferenceNullabilityTests : ArgumentativeFilterTests
{
    [Fact]
    public async Task NonNullableParameter_WhenMissingDuringCreation_CausesFilterFallback()
    {
        await TestFilterFactoryBehaviour<NonNullableParameterArgumentativeFilter>(
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
    
    [Fact]
    public async Task NonNrtParameter_WhenMissingDuringCreation_CausesFilterFallback()
    {
        await TestFilterFactoryBehaviour<NonNrtContextParameterFilter>(
            (string notTest) => notTest,
            async (context, filter, fallbackResult) => {
                // Act
                var result = await filter(context.InvocationContext);
        
                // Assert
                result.ShouldBe(fallbackResult);
            });
    }    
    
    [Fact]
    public async Task OptionalNonNrtParameter_WhenMissingDuringCreation_RunsFilter()
    {
        await TestFilterFactoryBehaviour<OptionalNonNrtContextParameterFilter>(
            (string notTest) => notTest,
            async (context, filter, _) => {
                // Act
                var result = await filter(context.InvocationContext);
        
                // Assert
                result.ShouldBeNull();
            });
    }
    
    public partial class NonNullableParameterArgumentativeFilter : IFilterFactory
    {
        [ArgumentativeFilter]
        public static ValueTask<object?> TestFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, string test)
        {
            return ValueTask.FromResult<object?>(null);
        }
    }
    
    public partial class NullableParameterFilter : IFilterFactory
    {
        [ArgumentativeFilter]
        public static ValueTask<object?> TestFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, string? test)
        {
            return ValueTask.FromResult<object?>(null);
        }
    }
    
    public partial class NonNrtContextParameterFilter : IFilterFactory
    {
#nullable disable
        [ArgumentativeFilter]
        public static ValueTask<object> TestFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, string test)
        {
            return ValueTask.FromResult<object>(null);
        }
#nullable restore
    }    
    
    public partial class OptionalNonNrtContextParameterFilter : IFilterFactory
    {
#nullable disable
        [ArgumentativeFilter]
        public static ValueTask<object> TestFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, string test = null)
        {
            return ValueTask.FromResult<object>(null);
        }
#nullable restore
    }
}