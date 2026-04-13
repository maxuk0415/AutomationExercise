using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// 封裝網站頂部導航列（header nav），幾乎所有頁面都有。
/// 集中管理導航 locator，避免每個 Page Object 重複定義相同的 selector。
/// </summary>
public class NavBarPage(IPage page)
{
    // --- Locators ---
    // 加上 header 父容器限制，避免與頁面上其他指向 "/" 的連結（例如 logo）混淆
    private ILocator HomeLink       => page.Locator("header a[href='/']");
    private ILocator ProductsLink   => page.Locator("a[href='/products']");
    private ILocator CartLink       => page.Locator("a[href='/view_cart']");
    private ILocator LoginLink      => page.Locator("a[href='/login']");
    private ILocator LogoutLink     => page.Locator("a[href='/logout']");
    private ILocator ContactUsLink  => page.Locator("a[href='/contact_us']");
    private ILocator LoggedInAsText => page.Locator("a:has-text('Logged in as')");

    // --- Methods ---
    public async Task ClickHomeAsync()
        => await HomeLink.ClickAsync();

    public async Task ClickProductsAsync()
        => await ProductsLink.ClickAsync();

    public async Task ClickCartAsync()
        => await CartLink.ClickAsync();

    public async Task ClickLoginAsync()
        => await LoginLink.ClickAsync();

    public async Task ClickLogoutAsync()
        => await LogoutLink.ClickAsync();

    public async Task ClickContactUsAsync()
        => await ContactUsLink.ClickAsync();

    public async Task<bool> IsLoggedInAsync()
    {
        try
        {
            // 等待元素出現（最多 10 秒），適用於登入後頁面跳轉的情況
            await LoggedInAsText.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> GetLoggedInUsernameAsync()
    {
        var text = await LoggedInAsText.InnerTextAsync();
        // 格式為 "Logged in as Max AutoTest"，取出名字部分
        return text.Replace("Logged in as ", "").Trim();
    }

    public async Task<bool> IsLoginLinkVisibleAsync()
        => await LoginLink.IsVisibleAsync();
}
