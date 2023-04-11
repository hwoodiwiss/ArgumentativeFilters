using Microsoft.AspNetCore.Http;

namespace ArgumentativeFilters.Integration.Tests.Behaviours;

public partial class IndexOfAttributeTests : ArgumentativeFilterTests
{
    [Theory]
    [MemberData(nameof(EndpointDelegatesWithExpectedParamIndex))]
    public async Task IndexOfAttribute_WhenAppliedToParameter_ShouldPassCorrectIndexToFilter(Delegate endpointSignature, int paramIndex)
    {
        // Arrange
        SetupContext(endpointSignature, new List<object?>());
        var filter = NonOptionalIndexFilter.Factory(Context.FactoryContext, NullEndpointFilterDelegate);
        
        // Act
        var result = ((await filter(Context.InvocationContext)) ?? int.MinValue);
        
        // Assert
        result.ShouldBe(paramIndex);
    }
    
    [Fact]
    public async Task IndexOfAttribute_WhenAppliedToNonOptionalParameter_AndParameterDoesNotExistOnEndpoint_ShouldFallBackToDefault()
    {
        // Arrange
        SetupContext((string test) => test, new List<object?>());
        var filter = NonOptionalIndexFilter.Factory(Context.FactoryContext, NullEndpointFilterDelegate);
        
        // Act
        var result = await filter(Context.InvocationContext);
        
        // Assert
        result.ShouldBe(null);
    }
    
    [Fact]
    public async Task IndexOfAttribute_WhenAppliedToOptionalParameter_AndParameterDoesNotExistOnEndpoint_ShouldStillCallFilter()
    {
        // Arrange
        SetupContext((string test) => test, new List<object?>());
        var filter = OptionalIndexFilter.Factory(Context.FactoryContext, NullEndpointFilterDelegate);
        
        // Act
        var result = await filter(Context.InvocationContext);
        
        // Assert
        result.ShouldBe(false);
    }
    
    [Fact]
    public async Task IndexOfAttribute_WhenAppliedToOptionalParameter_AndParameterDoesExistOnEndpoint_ShouldStillCallFilter()
    {
        // Arrange
        SetupContext((string expectedParam) => expectedParam, new List<object?>());
        var filter = OptionalIndexFilter.Factory(Context.FactoryContext, NullEndpointFilterDelegate);
        
        // Act
        var result = await filter(Context.InvocationContext);
        
        // Assert
        result.ShouldBe(true);
    }
    
    internal static partial class NonOptionalIndexFilter
    {
        [ArgumentativeFilter]
        private static ValueTask<object?> TestFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, [IndexOf("expectedParam")] int actualIndex)
        {
            return ValueTask.FromResult<object?>(actualIndex);
        }
    }
    
    internal static partial class OptionalIndexFilter
    {
        [ArgumentativeFilter]
        private static ValueTask<object?> TestFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, [IndexOf("expectedParam")] int? actualIndex)
        {
            return ValueTask.FromResult<object?>(actualIndex.HasValue);
        }
    }

    public static IEnumerable<object[]> EndpointDelegatesWithExpectedParamIndex() => new[]
    {
        new object[] {(string expectedParam, string someParam) => string.Empty, 0},
        new object[] {(string someParam, string expectedParam) => string.Empty, 1},
        new object[] {(string someParam, string anotherParam, string aFurtherParam, string anotherOne, string expectedParam) => string.Empty, 4},
    };
}