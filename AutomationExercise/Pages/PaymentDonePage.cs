using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// 負責付款完成頁面：確認訂單成功訊息。
/// </summary>
public class PaymentDonePage(IPage page)
{
    // --- Locators ---
    private ILocator SuccessMessage => page.Locator("h2[data-qa='order-placed']");

    // --- Methods ---
    public async Task<string> GetConfirmationMessageAsync()
        => await SuccessMessage.InnerTextAsync();

    public async Task<bool> IsOrderConfirmedAsync()
    {
        try
        {
            await SuccessMessage.WaitForAsync(new LocatorWaitForOptions { Timeout = 15000 });
            return true;
        }
        catch { return false; }
    }
}
