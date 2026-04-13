using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// 負責帳號刪除確認頁面（/delete_account）：確認刪除成功訊息。
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
