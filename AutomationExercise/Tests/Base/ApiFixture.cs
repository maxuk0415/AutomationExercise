using Microsoft.Playwright;

namespace AutomationExercise.Tests.Base;

/// <summary>
/// API 測試的 base class，使用 Playwright 的 APIRequestContext。
/// 不啟動 browser，比 E2E fixture 更輕量，適合純 HTTP 測試。
///
/// 用法：
///   public class ProductsApiTests : ApiFixture
///   {
///       [Fact]
///       public async Task ShouldReturnProductListWhenCallingProductsApi()
///       {
///           var response = await ApiContext.GetAsync("/api/productsList");
///           Assert.Equal(200, response.Status);
///       }
///   }
///
/// 重要：automationexercise.com 的 API 永遠回傳 HTTP 200，
/// 實際的成功/失敗狀態在 JSON body 的 responseCode 欄位（200=成功，400/404=失敗）。
/// 所以斷言時必須同時驗證 response.Status 和 responseCode，不能只驗證 HTTP status。
/// </summary>
public class ApiFixture : IAsyncLifetime
{
    private IPlaywright _playwright = null!;
    protected IAPIRequestContext ApiContext { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();

        ApiContext = await _playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = Urls.Base,
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                ["Accept"] = "application/json"
            }
        });
    }

    public async Task DisposeAsync()
    {
        if (ApiContext != null) await ApiContext.DisposeAsync();
        _playwright?.Dispose();
    }
}
