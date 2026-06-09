using BenchmarkDotNet.Attributes;

namespace ArgumentativeFilters.Benchmarks;

[MemoryDiagnoser]
public class FilterAndMiddlewareBenchmarks
{
    private BenchmarkWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [GlobalSetup]
    public void Setup()
    {
        _factory = new BenchmarkWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    /// <summary>
    /// Benchmarks a request that passes through the generated NormalizeRouteCountry filter,
    /// ValidateId filter, and AddMinimumAllowedIdHeader middleware, returning 200 OK.
    /// </summary>
    [Benchmark]
    public async Task<HttpResponseMessage> ValidRequest()
        => await _client.GetAsync(new Uri("/country/spain/500001", UriKind.Relative));

    /// <summary>
    /// Benchmarks a request rejected by the generated ValidateId filter, returning 400 Bad Request.
    /// </summary>
    [Benchmark]
    public async Task<HttpResponseMessage> InvalidIdRequest()
        => await _client.GetAsync(new Uri("/country/spain/1", UriKind.Relative));
}
