using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// Handles the /contact_us page: fill in and submit the contact form.
/// </summary>
public class ContactUsPage(IPage page)
{
    // --- Locators ---
    private ILocator NameInput      => page.Locator("input[data-qa='name']");
    private ILocator EmailInput     => page.Locator("input[data-qa='email']");
    private ILocator SubjectInput   => page.Locator("input[data-qa='subject']");
    private ILocator MessageInput   => page.Locator("textarea[data-qa='message']");
    private ILocator SubmitButton   => page.Locator("input[data-qa='submit-button']");
    private ILocator FileInput       => page.Locator("input[name='upload_file']");
    private ILocator HomeButton      => page.Locator("a:has-text('Home')").First;
    private ILocator SuccessMessage  => page.Locator("div.status.alert.alert-success");

    // --- Methods ---
    public async Task NavigateAsync()
        => await page.GotoAsync(Urls.Base + Urls.ContactUs);

    public async Task FillContactFormAsync(string name, string email, string subject, string message)
    {
        await NameInput.FillAsync(name);
        await EmailInput.FillAsync(email);
        await SubjectInput.FillAsync(subject);
        await MessageInput.FillAsync(message);
    }

    public async Task SubmitFormAsync()
    {
        // The dialog handler must be registered before clicking, otherwise the dialog may appear before the handler is attached
        // Using a named method so the handler removes itself after firing once, preventing interference with subsequent operations
        void Handler(object? _, IDialog dialog)
        {
            _ = dialog.AcceptAsync();
            page.Dialog -= Handler;
        }
        page.Dialog += Handler;
        await SubmitButton.ClickAsync();
    }

    public async Task<string> GetSuccessMessageAsync()
        => await SuccessMessage.InnerTextAsync();

    public async Task UploadFileAsync(string filePath)
        => await FileInput.SetInputFilesAsync(filePath);

    public async Task ClickHomeAsync()
    {
        await HomeButton.ScrollIntoViewIfNeededAsync();
        await HomeButton.ClickAsync();
        // WaitForFunctionAsync: polls in the browser until the URL has actually left the contact_us page
        // More reliable than NetworkIdle / WaitForLoadState: won't resolve immediately if the current page is already idle
        // More reliable than WaitForURLAsync(string): not sensitive to exact URL format differences (trailing slash, etc.)
        await page.WaitForFunctionAsync(
            "() => !window.location.href.includes('contact_us')",
            null,
            new PageWaitForFunctionOptions { Timeout = 20000 });
    }

    public async Task<bool> IsSuccessMessageVisibleAsync()
    {
        try
        {
            await SuccessMessage.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });
            return true;
        }
        catch { return false; }
    }
}
