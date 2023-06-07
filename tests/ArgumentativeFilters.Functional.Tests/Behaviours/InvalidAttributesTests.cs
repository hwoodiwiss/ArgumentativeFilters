using Microsoft.AspNetCore.Http;

namespace ArgumentativeFilters.Functional.Tests.Behaviours;

public partial class InvalidAttributesTests : ArgumentativeFilterTests
{
    const string FilterRunResult = nameof(InvalidAttributesTests);

    [Fact]
    public async Task IndexOfParameter_WhenParameterDoesNotExists_AndIndexParamIsNullable_RunsFilter()
    {
        await TestFilterFactoryBehaviour<NullableInvalidIndexOfFilter>(
            (string test) => test,
            async (context, filter, fallbackValue) =>
            {
                // Act
                var result = await filter(context.InvocationContext);

                // Assert
                result.ShouldBe(FilterRunResult);
            });
    }

    [Fact]
    public async Task IndexOfParameter_WhenParameterDoesNotExists_AndIndexParamIsNotNullable_CausesFilterFallback()
    {
        await TestFilterFactoryBehaviour<NonNullableInvalidIndexOfFilter>(
            (string test) => test,
            async (context, filter, fallbackValue) =>
            {
                // Act
                var result = await filter(context.InvocationContext);

                // Assert
                result.ShouldBe(fallbackValue);
            });
    }

    public partial class NullableInvalidIndexOfFilter : IFilterFactory
    {
        [ArgumentativeFilter]
        public static ValueTask<object?> TestFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, [IndexOf("badParam")] int? test)
        {
            return ValueTask.FromResult<object?>(FilterRunResult);
        }
    }

    public partial class NonNullableInvalidIndexOfFilter : IFilterFactory
    {
        [ArgumentativeFilter]
        public static ValueTask<object?> TestFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, [IndexOf("badParam")] int test)
        {
            return ValueTask.FromResult<object?>(FilterRunResult);
        }
    }
}
