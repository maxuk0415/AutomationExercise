using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// Handles the payment page: fill in credit card details and submit the order.
/// </summary>
public class PaymentPage(IPage page)
{
    // --- Locators ---
    private ILocator CardNameInput    => page.Locator("input[data-qa='name-on-card']");
    private ILocator CardNumberInput  => page.Locator("input[data-qa='card-number']");
    private ILocator CvcInput         => page.Locator("input[data-qa='cvc']");
    private ILocator ExpiryMonthInput => page.Locator("input[data-qa='expiry-month']");
    private ILocator ExpiryYearInput  => page.Locator("input[data-qa='expiry-year']");
    private ILocator PayButton        => page.Locator("button[data-qa='pay-button']");

    // --- Methods ---
    public async Task FillPaymentDetailsAsync(
        string cardName,
        string cardNumber,
        string cvc,
        string expiryMonth,
        string expiryYear)
    {
        // WaitForAsync: ensures the payment page has loaded (navigation after PlaceOrderAsync can be slower in WebKit)
        await CardNameInput.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 60000
        });
        await CardNameInput.FillAsync(cardName);
        await CardNumberInput.FillAsync(cardNumber);
        await CvcInput.FillAsync(cvc);
        await ExpiryMonthInput.FillAsync(expiryMonth);
        await ExpiryYearInput.FillAsync(expiryYear);
    }

    public async Task SubmitPaymentAsync()
        => await PayButton.ClickAsync();
}
