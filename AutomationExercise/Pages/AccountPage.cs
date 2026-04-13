using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// 負責登入後的帳號頁面（/account）：驗證登入狀態、刪除帳號。
/// </summary>
public class AccountPage(IPage page)
{
    // --- Locators ---
    // 注意：「Logged in as」是 NavBar 的元素，登入狀態驗證請使用 NavBarPage
    private ILocator DeleteAccountBtn => page.Locator("a[href='/delete_account']");
    private ILocator LogoutLink       => page.Locator("a[href='/logout']");

    // --- Methods ---
    public async Task DeleteAccountAsync()
        => await DeleteAccountBtn.ClickAsync();

    public async Task LogoutAsync()
        => await LogoutLink.ClickAsync();
}
