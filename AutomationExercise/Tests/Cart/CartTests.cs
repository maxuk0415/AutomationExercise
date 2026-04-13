using AutomationExercise.Pages;
using AutomationExercise.Tests.Base;
using Microsoft.Playwright;

namespace AutomationExercise.Tests.Cart;

/// <summary>
/// 測試購物車功能：加入商品、顯示商品資訊、刪除商品、未登入結帳提示。
///
/// SetUp 策略：private helper method
/// 原因：各 test 需要不同起始狀態
///   - 驗證加入購物車 → 需要從 Products 頁加商品
///   - 驗證刪除商品   → 需要先加商品，再到 Cart 刪除
///   - 未登入結帳     → 不需要登入，直接到 Cart 點結帳
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

        // 等待商品列表更新後確認購物車為空
        await Page.WaitForSelectorAsync("#cart_info_table tbody tr",
            new PageWaitForSelectorOptions { State = WaitForSelectorState.Hidden, Timeout = 5000 });

        Assert.True(await cartPage.IsCartEmptyAsync());
    }

    [Fact]
    public async Task ShouldShowLoginModalWhenCheckoutAsGuest()
    {
        // 不登入，先加商品（Checkout 按鈕只有購物車有商品時才會出現）
        await AddFirstProductToCartAsync();

        var cartPage = new CartPage(Page);
        await cartPage.NavigateAsync();
        await cartPage.ClickCheckoutAsync();

        Assert.True(await cartPage.IsLoginModalVisibleAsync());
    }
}
