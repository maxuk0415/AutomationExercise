using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// Handles the post-login account page (/account): verifying login state and deleting the account.
/// </summary>
public class AccountPage(IPage page)
{
    // --- Locators ---
    // Note: "Logged in as" is a NavBar element — use NavBarPage to verify login state
    private ILocator DeleteAccountBtn => page.Locator("a[href='/delete_account']");
    private ILocator LogoutLink       => page.Locator("a[href='/logout']");

    // --- Methods ---
    public async Task DeleteAccountAsync()
        => await DeleteAccountBtn.ClickAsync();

    public async Task LogoutAsync()
        => await LogoutLink.ClickAsync();
}
