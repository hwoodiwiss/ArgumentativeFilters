using System.Net;

namespace ExampleMinimalApi.Integration.Tests.Middleware;

public class MinimumAllowedIdHeaderMiddlewareTests : IClassFixture<ExampleMinimalApiFixture>
{
    private const string HeaderName = "X-Minimum-Allowed-Id";
    private readonly HttpClient _client;

    public MinimumAllowedIdHeaderMiddlewareTests(ExampleMinimalApiFixture fixture)
    {
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task MinimumAllowedIdHeaderMiddleware_AddsConfiguredHeaderToResponse()
    {
        // Act
        var response = await _client.GetAsync(new Uri("country/spain/500000", UriKind.Relative), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.TryGetValues(HeaderName, out var values).ShouldBeTrue();
        values.ShouldNotBeNull();
        values.ShouldHaveSingleItem().ShouldBe("500000");
    }
}
