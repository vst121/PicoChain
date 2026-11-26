using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace PicoChain.Tests;

public sealed class ApiHealthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public ApiHealthTests(WebApplicationFactory<Program> factory)
    {
        client = factory.WithWebHostBuilder(builder => builder.ConfigureLogging(logging => logging.ClearProviders())).CreateClient();
    }

    [Fact]
    public async Task Health_endpoint_returns_ok()
    {
        using var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("picochain-api", await response.Content.ReadAsStringAsync());
    }
}
