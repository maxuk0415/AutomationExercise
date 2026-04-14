using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// 負責 /contact_us 頁面：填寫並送出聯絡表單。
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
        // 必須在 Click 之前掛上 Dialog handler，否則 dialog 可能在 handler 註冊前就出現
        // 使用具名方法讓 handler 在執行一次後自動移除，避免影響後續操作
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
        // WaitForLoadStateAsync 在 WebKit 會立刻 resolve（當前頁面已 loaded）→ URL 尚未變更
        // 改用 WaitForURLAsync 等 URL 確實變為首頁後再繼續
        await page.WaitForURLAsync(Urls.Base + "/",
            new PageWaitForURLOptions { Timeout = 10000 });
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
