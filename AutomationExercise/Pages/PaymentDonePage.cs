using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// Handles the payment confirmation page: verify order success message.
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
