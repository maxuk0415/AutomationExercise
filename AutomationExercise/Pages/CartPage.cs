using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// 負責 /view_cart 頁面：查看商品、刪除商品、前往結帳。
/// </summary>
public class CartPage(IPage page)
{
    // --- Locators ---
    private ILocator CartRows        => page.Locator("#cart_info_table tbody tr");
    private ILocator CheckoutButton  => page.Locator("a.btn.btn-default.check_out");
    private ILocator LoginModal      => page.Locator("#checkoutModal");
    private ILocator LoginModalLink  => page.Locator("#checkoutModal a:text('Login')");

    // --- Methods ---
    public async Task NavigateAsync()
        => await page.GotoAsync(Urls.Base + Urls.Cart);

    public async Task<int> GetCartItemCountAsync()
        => await CartRows.CountAsync();

    public async Task<string> GetProductNameInCartAsync(int rowIndex = 0)
        => await CartRows.Nth(rowIndex).Locator("td.cart_description h4 a").InnerTextAsync();

    public async Task<string> GetProductPriceInCartAsync(int rowIndex = 0)
        => await CartRows.Nth(rowIndex).Locator("td.cart_price p").InnerTextAsync();

    public async Task<string> GetProductQuantityInCartAsync(int rowIndex = 0)
        => await CartRows.Nth(rowIndex).Locator("td.cart_quantity button").InnerTextAsync();

    public async Task RemoveProductAsync(int rowIndex = 0)
        => await CartRows.Nth(rowIndex).Locator("td.cart_delete a").ClickAsync();

    public async Task ClickCheckoutAsync()
    {
        await CheckoutButton.ScrollIntoViewIfNeededAsync();
        await CheckoutButton.ClickAsync();
    }

    public async Task<bool> IsLoginModalVisibleAsync()
        => await LoginModal.IsVisibleAsync();

    // 用於負向測試：先確認購物車是否為空，避免操作空購物車時 timeout
    public async Task<bool> IsCartEmptyAsync()
        => await CartRows.CountAsync() == 0;
}
