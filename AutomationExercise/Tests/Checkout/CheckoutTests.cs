using AutomationExercise.Pages;
using AutomationExercise.Tests.Base;

namespace AutomationExercise.Tests.Checkout;

/// <summary>
/// Tests the checkout flow: order summary, payment completion.
///
/// SetUp strategy: private helper method
/// Reason: all tests share the same precondition (logged in + item in cart + on checkout page),
/// but each test performs different actions after reaching checkout, so a helper is used instead of overriding InitializeAsync.
/// </summary>
public class CheckoutTests : PlaywrightFixture, IClassFixture<BrowserFixture>
{
    public CheckoutTests(BrowserFixture browser) : base(browser) { }

    /// <summary>
    /// Shared setup: login → add item → navigate to Checkout page
    /// </summary>
    private async Task SetupCheckoutAsync()
    {
        await LoginAsync();

        var productsPage = new ProductsPage(Page);
        await productsPage.NavigateAsync();
        await DismissConsentDialogAsync(); // consent may reappear after each Navigate in WebKit
        await productsPage.AddFirstProductToCartAsync();

        var cartPage = new CartPage(Page);
        await cartPage.NavigateAsync();
        await DismissConsentDialogAsync(); // ensure checkout button is not obscured
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
