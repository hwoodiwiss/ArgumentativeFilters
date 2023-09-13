using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ExampleMinimalApi.Integration.Tests.Filters;

public class ValidateIdFilterTests : IClassFixture<ExampleMinimalApiFixture>
{
    private readonly HttpClient _client;

    public ValidateIdFilterTests(ExampleMinimalApiFixture fixture)
    {
        _client = fixture.CreateClient();
    }
    
    [Fact]
    public async Task ValidateIdFilter_WhenIdIsValid_WillLetEndpointReturnOkWithIdInResponse()
    {
        // Arrange
        const int validId = 500000;
        
        // Act
        var response = await _client.GetAsync(new Uri($"country/spain/{validId}", UriKind.Relative));

        // Assert
        var resptext = await response.Content.ReadAsStringAsync();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var exampleResponse = await response.Content.ReadFromJsonAsync<ExampleResponse>();
        exampleResponse.ShouldNotBeNull();
        exampleResponse.Id.ShouldBe(validId);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(100000)]
    [InlineData(499999)]
    public async Task ValidateIdFilter_WhenIdIsInvalid_WillReturnBadRequest(int invalidId)
    {
        // Act
        var response = await _client.GetAsync(new Uri($"country/wales/{invalidId}", UriKind.Relative));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}