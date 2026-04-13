using Microsoft.Playwright;
using System.Text.Json;
using AutomationExercise.Tests.Base;

namespace AutomationExercise.Tests.Api;

/// <summary>
/// API 測試：使用 Playwright 的 APIRequestContext（不啟動 browser）。
///
/// 重要設計說明：
///   automationexercise.com 的 API 永遠回傳 HTTP 200，
///   實際結果在 JSON body 的 responseCode 欄位（200=成功，400=缺少參數，404=帳號不存在，405=Method Not Allowed）。
///   因此每個測試必須同時驗證 HTTP status（永遠是 200）和 responseCode（實際語意）。
///
/// 用 System.Text.Json 解析 JSON body，避免額外安裝套件。
/// </summary>
public class ApiTests : ApiFixture
{
    // ──────────────────────────────────────────
    // 輔助方法：解析 JSON body 取出 responseCode
    // ──────────────────────────────────────────

    /// <summary>
    /// 從 response body 的 JSON 解析出 responseCode 欄位。
    /// </summary>
    private static async Task<int> GetResponseCodeAsync(IAPIResponse response)
    {
        var body = await response.TextAsync();
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("responseCode").GetInt32();
    }

    /// <summary>
    /// 從 response body 的 JSON 解析出某個陣列欄位的元素數量。
    /// 例如 products → "products" array，brands → "brands" array。
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

        Assert.Equal(200, response.Status); // 網站永遠回傳 HTTP 200
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
