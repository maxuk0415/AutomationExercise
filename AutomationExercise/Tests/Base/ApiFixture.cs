using Microsoft.Playwright;

namespace AutomationExercise.Tests.Base;

/// <summary>
/// Base class for API tests using Playwright's APIRequestContext.
/// Does not launch a browser — lighter weight than the E2E fixture, suited for pure HTTP testing.
///
/// Usage:
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
/// Important: automationexercise.com APIs always return HTTP 200.
/// The actual success/failure status is in the JSON body's responseCode field (200=success, 400/404=failure).
/// Assertions must therefore validate both response.Status and responseCode — never just the HTTP status alone.
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
