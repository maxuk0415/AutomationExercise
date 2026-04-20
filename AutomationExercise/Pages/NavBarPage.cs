using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// Encapsulates the site's top navigation bar (header nav), present on almost every page.
/// Centralises navigation locators to avoid redefining the same selectors across Page Objects.
/// </summary>
public class NavBarPage(IPage page)
{
    // --- Locators ---
    // Scoped to the header parent to avoid ambiguity with other links pointing to "/" (e.g. logo)
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
            // Wait for element to appear (up to 10 seconds), suitable for post-login page transitions
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
        // Format is "Logged in as Max AutoTest" — extract the name portion
        return text.Replace("Logged in as ", "").Trim();
    }

    public async Task<bool> IsLoginLinkVisibleAsync()
        => await LoginLink.IsVisibleAsync();
}
