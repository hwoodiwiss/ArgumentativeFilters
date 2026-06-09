extern alias ExampleMinimalApiApp;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace ArgumentativeFilters.Benchmarks;

public sealed class BenchmarkWebApplicationFactory : WebApplicationFactory<ExampleMinimalApiApp::Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(cfg =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MinimumAllowedId"] = "500000",
            });
        });

        base.ConfigureWebHost(builder);
    }
}
