using AutomationExercise.Pages;
using AutomationExercise.Tests.Base;

namespace AutomationExercise.Tests.Checkout;

/// <summary>
/// 測試結帳流程：訂單摘要、付款完成。
///
/// SetUp 策略：private helper method
/// 原因：所有 test 都需要「已登入 + 有商品在購物車 + 已進入 checkout 頁」，
/// 但不同 test 在 checkout 後的操作不同，所以用 helper 而非 override InitializeAsync。
/// </summary>
public class CheckoutTests : PlaywrightFixture, IClassFixture<BrowserFixture>
{
    public CheckoutTests(BrowserFixture browser) : base(browser) { }

    /// <summary>
    /// 共用前置步驟：登入 → 加商品 → 進入 Checkout 頁
    /// </summary>
    private async Task SetupCheckoutAsync()
    {
        await LoginAsync();

        var productsPage = new ProductsPage(Page);
        await productsPage.NavigateAsync();
        await productsPage.AddFirstProductToCartAsync();

        var cartPage = new CartPage(Page);
        await cartPage.NavigateAsync();
        await cartPage.ClickCheckoutAsync();
    }

    [Fact]
    public async Task ShouldNavigateToCheckoutPageWhenProceedingFromCart()
    {
        await SetupCheckoutAsync();

        Assert.Contains(Urls.Checkout, Page.Url);
    }

    [Fact]
    public async Task ShouldDisplayOrderedProductInOrderSummary()
    {
        await SetupCheckoutAsync();

        var checkoutPage = new CheckoutPage(Page);
        var itemCount = await checkoutPage.GetOrderItemCountAsync();
        Assert.True(itemCount > 0);
    }

    [Fact]
    public async Task ShouldPlaceOrderSuccessfullyWhenPaymentDetailsAreValid()
    {
        await SetupCheckoutAsync();

        var checkoutPage = new CheckoutPage(Page);
        await checkoutPage.PlaceOrderAsync();

        var paymentPage = new PaymentPage(Page);
        await paymentPage.FillPaymentDetailsAsync(
            cardName:    PaymentData.CardName,
            cardNumber:  PaymentData.CardNumber,
            cvc:         PaymentData.Cvc,
            expiryMonth: PaymentData.ExpiryMonth,
            expiryYear:  PaymentData.ExpiryYear);
        await paymentPage.SubmitPaymentAsync();

        var donePage = new PaymentDonePage(Page);
        Assert.True(await donePage.IsOrderConfirmedAsync());
    }
}
