using Microsoft.AspNetCore.Http;

namespace ArgumentativeFilters.Functional.Tests.Behaviours;

public partial class RefParameterTests : ArgumentativeFilterTests
{
    const string FilterRunResult = nameof(RefParameterTests);
    
    [Fact]
    public async Task RefParameterFilter_WhenParameterDoesNotExists_AndRefParamIsNullable_RunsFilter()
    {
        await TestFilterFactoryBehaviour<RefParameterFilter>(
            (int test) => test,
            async (context, filter, _) =>
            {
                // Act
                var result = await filter(context.InvocationContext);

                // Assert
                result.ShouldBe(FilterRunResult);
            }, 
            new List<object?> { 0 });
    }

    [Fact]
    public async Task InParameterFilter_WhenParameterDoesNotExists_AndRefParamIsNotNullable_CausesFilterFallback()
    {
        await TestFilterFactoryBehaviour<InParameterFilter>(
            (int test) => test,
            async (context, filter, _) =>
            {
                // Act
                var result = await filter(context.InvocationContext);

                // Assert
                result.ShouldBe(FilterRunResult);
            },
            new List<object?> { 0 });
    }    
    
    [Fact]
    public async Task OutParameterFilter_WhenParameterDoesNotExists_AndRefParamIsNotNullable_CausesFilterFallback()
    {
        await TestFilterFactoryBehaviour<OutParameterFilter>(
            (int test) => test,
            async (context, filter, _) =>
            {
                // Act
                var result = await filter(context.InvocationContext);

                // Assert
                result.ShouldBe(FilterRunResult);
            },
            new List<object?> { 0 });
    }

    public partial class RefParameterFilter : IFilterFactory
    {
        [ArgumentativeFilter]
        public static ValueTask<object?> TestFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, ref int test)
        {
            return ValueTask.FromResult<object?>(FilterRunResult);
        }
    }
    
    public partial class InParameterFilter : IFilterFactory
    {
        [ArgumentativeFilter]
        public static ValueTask<object?> TestFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, in int test)
        {
            return ValueTask.FromResult<object?>(FilterRunResult);
        }
    }
    
    // Not sure why you would, but...
    public partial class OutParameterFilter : IFilterFactory
    {
        [ArgumentativeFilter]
        public static ValueTask<object?> TestFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, out int test)
        {
            test = int.MaxValue;
            return ValueTask.FromResult<object?>(FilterRunResult);
        }
    }
}