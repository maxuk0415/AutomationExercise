using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// Handles the /view_cart page: view items, remove items, proceed to checkout.
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

    // Used for negative tests: check whether the cart is empty before operating on an empty cart to avoid timeouts
    public async Task<bool> IsCartEmptyAsync()
        => await CartRows.CountAsync() == 0;
}
