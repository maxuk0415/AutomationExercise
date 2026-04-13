using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// 負責單一產品詳情頁面：取得資訊、設定數量、加入購物車。
/// </summary>
public class ProductDetailPage(IPage page)
{
    // --- Locators ---
    private ILocator ProductName   => page.Locator(".product-information h2");
    private ILocator ProductPrice  => page.Locator(".product-information span span");
    private ILocator QuantityInput => page.Locator("input#quantity");
    // 加上父容器限制，避免頁面上其他「Add to cart」按鈕造成 strict mode violation
    private ILocator AddToCartBtn  => page.Locator(".product-information button:text('Add to cart')");
    private ILocator ViewCartLink  => page.Locator("u:text('View Cart')");

    // --- Methods ---
    public async Task<string> GetProductNameAsync()
    {
        await ProductName.WaitForAsync(new LocatorWaitForOptions { Timeout = 15000 });
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
