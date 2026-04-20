using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// Handles the account deletion confirmation page (/delete_account): verify deletion success message.
/// </summary>
public class AccountDeletedPage(IPage page)
{
    // --- Locators ---
    private ILocator AccountDeletedMsg => page.Locator("h2[data-qa='account-deleted']");
    private ILocator ContinueButton    => page.Locator("a[data-qa='continue-button']");

    // --- Methods ---
    public async Task<string> GetAccountDeletedMessageAsync()
        => await AccountDeletedMsg.InnerTextAsync();

    public async Task<bool> IsAccountDeletedAsync()
        => await AccountDeletedMsg.IsVisibleAsync();

    public async Task ClickContinueAsync()
        => await ContinueButton.ClickAsync();
}
