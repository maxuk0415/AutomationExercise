using AutomationExercise.Pages;
using AutomationExercise.Tests.Base;
using Microsoft.Playwright;

namespace AutomationExercise.Tests.Cart;

/// <summary>
/// Tests cart functionality: adding items, displaying product info, removing items, checkout prompt for guests.
///
/// SetUp strategy: private helper method
/// Reason: each test requires a different starting state
///   - Verify add to cart → need to add an item from the Products page
///   - Verify remove item  → need to add an item first, then remove it from Cart
///   - Guest checkout      → no login needed; just add an item and click checkout
/// </summary>
public class CartTests : PlaywrightFixture, IClassFixture<BrowserFixture>
{
    public CartTests(BrowserFixture browser) : base(browser) { }

    private async Task AddFirstProductToCartAsync()
    {
        var productsPage = new ProductsPage(Page);
        await productsPage.NavigateAsync();
        await DismissConsentDialogAsync();
        await productsPage.AddFirstProductToCartAsync();
    }

    [Fact]
    public async Task ShouldAddProductToCartWhenClickingAddToCart()
    {
        await AddFirstProductToCartAsync();

        var cartPage = new CartPage(Page);
        await cartPage.NavigateAsync();

        Assert.False(await cartPage.IsCartEmptyAsync());
    }

    [Fact]
    public async Task ShouldDisplayCorrectProductInfoInCart()
    {
        await AddFirstProductToCartAsync();

        var cartPage = new CartPage(Page);
        await cartPage.NavigateAsync();

        var productName = await cartPage.GetProductNameInCartAsync();
        Assert.False(string.IsNullOrEmpty(productName));

        var price = await cartPage.GetProductPriceInCartAsync();
        Assert.Contains("Rs.", price);
    }

    [Fact]
    public async Task ShouldRemoveProductFromCartWhenClickingDelete()
    {
        await AddFirstProductToCartAsync();

        var cartPage = new CartPage(Page);
        await cartPage.NavigateAsync();
        await cartPage.RemoveProductAsync();

        // Wait for the product list to update, then confirm the cart is empty
        await Page.WaitForSelectorAsync("#cart_info_table tbody tr",
            new PageWaitForSelectorOptions { State = WaitForSelectorState.Hidden, Timeout = 5000 });

        Assert.True(await cartPage.IsCartEmptyAsync());
    }

    [Fact]
    public async Task ShouldShowLoginModalWhenCheckoutAsGuest()
    {
        // Not logged in — add an item first (the Checkout button only appears when the cart has items)
        await AddFirstProductToCartAsync();

        var cartPage = new CartPage(Page);
        await cartPage.NavigateAsync();
        await cartPage.ClickCheckoutAsync();

        Assert.True(await cartPage.IsLoginModalVisibleAsync());
    }
}
