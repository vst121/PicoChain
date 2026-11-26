using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PicoChain.Tests;

public sealed class ApiHealthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public ApiHealthTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_endpoint_returns_ok()
    {
        using var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("picochain-api", await response.Content.ReadAsStringAsync());
    }
}
