using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PicoChain.Tests;

public sealed class ApiContractTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public ApiContractTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Shipment_list_returns_json()
    {
        using var response = await client.GetAsync("/api/v1/shipments");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Pending_approvals_returns_json()
    {
        using var response = await client.GetAsync("/api/v1/approvals/pending");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }
}
