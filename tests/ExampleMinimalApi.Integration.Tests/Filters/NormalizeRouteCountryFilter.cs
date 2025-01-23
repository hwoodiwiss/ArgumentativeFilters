using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ExampleMinimalApi.Integration.Tests.Filters;

public class NormalizeRouteCountryFilter : IClassFixture<ExampleMinimalApiFixture>
{
    private readonly HttpClient _client;

    public NormalizeRouteCountryFilter(ExampleMinimalApiFixture fixture)
    {
        _client = fixture.CreateClient();
    }

    [Theory]
    [InlineData("united kingdom", "UNITED KINGDOM")]
    [InlineData("spain", "SPAIN")]
    [InlineData("usa", "USA")]
    [InlineData("france", "FRANCE")]
    public async Task NormalizeRouteCountryFilter_WhenCountryParameterIsNotUpperCase_WillReturnOkWithUppercaseCountryInResponse(string lowercaseCountry, string expectedCountryResponse)
    {
        // Act
        var response = await _client.GetAsync(new Uri($"country/{lowercaseCountry}/500000", UriKind.Relative), TestContext.Current.CancellationToken);
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var exampleResponse = await response.Content.ReadFromJsonAsync<ExampleResponse>(TestContext.Current.CancellationToken);
        exampleResponse.ShouldNotBeNull();
        exampleResponse.Country.ShouldBe(expectedCountryResponse);
    }
}