using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// 負責付款頁面：填入信用卡資料並送出訂單。
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
        // WaitForAsync：確保付款頁面已載入（PlaceOrderAsync 點擊後導航可能在 WebKit 較慢）
        await CardNameInput.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30000
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
