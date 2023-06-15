using Microsoft.AspNetCore.Http;

namespace ArgumentativeFilters.Functional.Tests.Behaviours;

public partial class IndexOfAttributeTests : ArgumentativeFilterTests
{
    const string FilterRunResult = nameof(IndexOfAttributeTests);

    [Theory]
    [MemberData(nameof(EndpointDelegatesWithExpectedParamIndex))]
    public async Task IndexOfAttribute_WhenAppliedToParameter_ShouldPassCorrectIndexToFilter(Delegate endpointSignature, int paramIndex)
    {
        await TestFilterFactoryBehaviour<NonOptionalIndexFilter>(
            endpointSignature,
            async (context, filter, _) =>
            {
                // Act
                var result = await filter(context.InvocationContext);

                // Assert
                result.ShouldBe(paramIndex);
            });
    }

    [Fact]
    public async Task IndexOfAttribute_WhenAppliedToNonOptionalParameter_AndParameterDoesNotExistOnEndpoint_ShouldFallBackToDefault()
    {
        await TestFilterFactoryBehaviour<NonOptionalIndexFilter>(
            (string test) => test,
            async (context, filter, fallbackResult) =>
            {
                // Act
                var result = await filter(context.InvocationContext);

                // Assert
                result.ShouldBe(fallbackResult);
            });
    }

    [Fact]
    public async Task IndexOfAttribute_WhenAppliedToOptionalParameter_AndParameterDoesNotExistOnEndpoint_ShouldStillCallFilter()
    {
        await TestFilterFactoryBehaviour<OptionalIndexFilter>(
            (string test) => test,
            async (context, filter, _) =>
            {
                // Act
                var result = await filter(context.InvocationContext);

                // Assert
                result.ShouldBe(FilterRunResult);
            });
    }

    [Fact]
    public async Task IndexOfAttribute_WhenAppliedToOptionalParameter_AndParameterDoesExistOnEndpoint_ShouldCallFilter()
    {
        await TestFilterFactoryBehaviour<OptionalIndexFilter>(
            (string expectedParam) => expectedParam,
            async (context, filter, _) =>
            {
                // Act
                var result = await filter(context.InvocationContext);

                // Assert
                result.ShouldBe(0);
            });
    }

    public partial class NonOptionalIndexFilter : IFilterFactory
    {
        [ArgumentativeFilter]
        private static ValueTask<object?> TestFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, [IndexOf("expectedParam")] int actualIndex)
        {
            return ValueTask.FromResult<object?>(actualIndex);
        }
    }

    public partial class OptionalIndexFilter : IFilterFactory
    {
        [ArgumentativeFilter]
        private static ValueTask<object?> TestFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, [IndexOf("expectedParam")] int? actualIndex)
        {
            return ValueTask.FromResult<object?>(actualIndex.HasValue ? (object)actualIndex.Value : FilterRunResult);
        }
    }

    public static IEnumerable<object[]> EndpointDelegatesWithExpectedParamIndex() => new[]
    {
        new object[] {(string expectedParam, string someParam) => string.Empty, 0},
        new object[] {(string someParam, string expectedParam) => string.Empty, 1},
        new object[] {(string someParam, string anotherParam, string aFurtherParam, string anotherOne, string expectedParam) => string.Empty, 4},
    };
}