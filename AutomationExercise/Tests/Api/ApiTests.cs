using Microsoft.Playwright;
using System.Text.Json;
using AutomationExercise.Tests.Base;

namespace AutomationExercise.Tests.Api;

/// <summary>
/// API tests using Playwright's APIRequestContext (no browser launched).
///
/// Important design note:
///   automationexercise.com APIs always return HTTP 200.
///   The actual result is in the JSON body's responseCode field (200=success, 400=missing params, 404=account not found, 405=Method Not Allowed).
///   Each test must therefore assert both the HTTP status (always 200) and responseCode (actual semantic result).
///
/// System.Text.Json is used to parse JSON responses — no additional packages required.
/// </summary>
public class ApiTests : ApiFixture
{
    // ──────────────────────────────────────────
    // Helper methods: parse responseCode from JSON body
    // ──────────────────────────────────────────

    /// <summary>
    /// Parses the responseCode field from the response body JSON.
    /// </summary>
    private static async Task<int> GetResponseCodeAsync(IAPIResponse response)
    {
        var body = await response.TextAsync();
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("responseCode").GetInt32();
    }

    /// <summary>
    /// Parses the element count of a named array field from the response body JSON.
    /// For example: products → "products" array, brands → "brands" array.
    /// </summary>
    private static async Task<int> GetArrayCountAsync(IAPIResponse response, string arrayKey)
    {
        var body = await response.TextAsync();
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty(arrayKey).GetArrayLength();
    }

    // ──────────────────────────────────────────
    // Products API
    // ──────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnProductsListWhenCallingGetProductsApi()
    {
        var response = await ApiContext.GetAsync("/api/productsList");

        Assert.Equal(200, response.Status);
        Assert.Equal(200, await GetResponseCodeAsync(response));
        Assert.True(await GetArrayCountAsync(response, "products") > 0,
            "Products list should contain at least one item");
    }

    [Fact]
    public async Task ShouldReturn405WhenPostingToProductsListApi()
    {
        var response = await ApiContext.PostAsync("/api/productsList", null);

        Assert.Equal(200, response.Status); // site always returns HTTP 200
        Assert.Equal(405, await GetResponseCodeAsync(response));
    }

    // ──────────────────────────────────────────
    // Brands API
    // ──────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnBrandsListWhenCallingGetBrandsApi()
    {
        var response = await ApiContext.GetAsync("/api/brandsList");

        Assert.Equal(200, response.Status);
        Assert.Equal(200, await GetResponseCodeAsync(response));
        Assert.True(await GetArrayCountAsync(response, "brands") > 0,
            "Brands list should contain at least one item");
    }

    [Fact]
    public async Task ShouldReturn405WhenPuttingToBrandsListApi()
    {
        var response = await ApiContext.PutAsync("/api/brandsList", null);

        Assert.Equal(200, response.Status);
        Assert.Equal(405, await GetResponseCodeAsync(response));
    }

    // ──────────────────────────────────────────
    // Search Product API
    // ──────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnSearchResultsWhenSearchingWithValidKeyword()
    {
        var formData = ApiContext.CreateFormData();
        formData.Set("search_product", "top");

        var response = await ApiContext.PostAsync("/api/searchProduct", new APIRequestContextOptions
        {
            Form = formData
        });

        Assert.Equal(200, response.Status);
        Assert.Equal(200, await GetResponseCodeAsync(response));
        Assert.True(await GetArrayCountAsync(response, "products") > 0,
            "Search results should contain at least one product for keyword 'top'");
    }

    [Fact]
    public async Task ShouldReturn400WhenSearchingWithoutKeyword()
    {
        var response = await ApiContext.PostAsync("/api/searchProduct", null);

        Assert.Equal(200, response.Status);
        Assert.Equal(400, await GetResponseCodeAsync(response));
    }

    // ──────────────────────────────────────────
    // Verify Login API
    // ──────────────────────────────────────────

    [Fact]
    public async Task ShouldReturn200WhenVerifyLoginWithValidCredentials()
    {
        var formData = ApiContext.CreateFormData();
        formData.Set("email", Users.ValidEmail);
        formData.Set("password", Users.ValidPassword);

        var response = await ApiContext.PostAsync("/api/verifyLogin", new APIRequestContextOptions
        {
            Form = formData
        });

        Assert.Equal(200, response.Status);
        Assert.Equal(200, await GetResponseCodeAsync(response));
    }

    [Fact]
    public async Task ShouldReturn404WhenVerifyLoginWithUnregisteredEmail()
    {
        var formData = ApiContext.CreateFormData();
        formData.Set("email", "nonexistent_user@example.com");
        formData.Set("password", "wrongpassword");

        var response = await ApiContext.PostAsync("/api/verifyLogin", new APIRequestContextOptions
        {
            Form = formData
        });

        Assert.Equal(200, response.Status);
        Assert.Equal(404, await GetResponseCodeAsync(response));
    }
}
