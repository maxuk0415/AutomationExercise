using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// 負責首頁（/）：輪播、特色商品、訂閱功能。
/// 導航列操作請使用 NavBarPage。
/// </summary>
public class HomePage(IPage page)
{
    // --- Locators ---
    private ILocator SubscriptionEmailInput => page.Locator("input#susbscribe_email");
    private ILocator SubscribeButton        => page.Locator("button#subscribe");
    private ILocator SubscriptionSuccess    => page.Locator("div#success-subscribe");
    private ILocator FeaturedProducts       => page.Locator(".features_items .productinfo");

    // --- Methods ---
    public async Task NavigateAsync()
        => await page.GotoAsync(Urls.Base);

    public async Task SubscribeAsync(string email)
    {
        await SubscriptionEmailInput.FillAsync(email);
        await SubscribeButton.ClickAsync();
    }

    public async Task<bool> IsSubscriptionSuccessVisibleAsync()
        => await SubscriptionSuccess.IsVisibleAsync();

    public async Task<string> GetSubscriptionSuccessMessageAsync()
        => await SubscriptionSuccess.InnerTextAsync();

    public async Task<int> GetFeaturedProductCountAsync()
        => await FeaturedProducts.CountAsync();
}
