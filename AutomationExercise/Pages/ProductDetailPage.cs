using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// Handles a single product detail page: retrieve info, set quantity, add to cart.
/// </summary>
public class ProductDetailPage(IPage page)
{
    // --- Locators ---
    private ILocator ProductName   => page.Locator(".product-information h2");
    private ILocator ProductPrice  => page.Locator(".product-information span span");
    private ILocator QuantityInput => page.Locator("input#quantity");
    // Scoped to parent container to avoid strict mode violations from other "Add to cart" buttons on the page
    private ILocator AddToCartBtn  => page.Locator(".product-information button:text('Add to cart')");
    private ILocator ViewCartLink  => page.Locator("u:text('View Cart')");

    // --- Methods ---
    public async Task<string> GetProductNameAsync()
    {
        // 30s timeout: WebKit CI renders the product detail page more slowly (DismissConsentDialog consumes extra time)
        await ProductName.WaitForAsync(new LocatorWaitForOptions { Timeout = 30000 });
        return await ProductName.InnerTextAsync();
    }

    public async Task<string> GetProductPriceAsync()
        => await ProductPrice.InnerTextAsync();

    public async Task SetQuantityAsync(int quantity)
    {
        await QuantityInput.ClearAsync();
        await QuantityInput.FillAsync(quantity.ToString());
    }

    public async Task AddToCartAsync()
        => await AddToCartBtn.ClickAsync();

    public async Task GoToCartAsync()
        => await ViewCartLink.ClickAsync();
}
