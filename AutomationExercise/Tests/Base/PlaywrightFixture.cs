using Microsoft.Playwright;
using AutomationExercise.Pages;

namespace AutomationExercise.Tests.Base;

/// <summary>
/// xUnit 版的 Playwright base class。
/// 實作 IAsyncLifetime 讓 xUnit 在每個 [Fact] 前後自動呼叫 InitializeAsync / DisposeAsync。
///
/// 架構設計：
///   - Browser 層：由 BrowserFixture 管理，整個 test class 只啟動一次（透過 IClassFixture）
///   - BrowserContext 層：每個 [Fact] 建立獨立的 context，確保 cookie/session 互不影響
///   - Page 層：每個 [Fact] 有自己的 Page
///
/// 效能優化：
///   舊架構：每個 test 啟動一次瀏覽器（~1-3 秒 × N 個 test）
///   新架構：整個 class 只啟動一次瀏覽器，每個 test 只建立 context（~50ms）
///
/// 用法：
///   public class LoginTests : PlaywrightFixture
///   {
///       public LoginTests(BrowserFixture browser) : base(browser) { }
///   }
/// </summary>
public class PlaywrightFixture : IAsyncLifetime
{
    private readonly BrowserFixture _browserFixture;
    private IBrowserContext _context = null!;

    protected IPage Page { get; private set; } = null!;

    public PlaywrightFixture(BrowserFixture browserFixture)
    {
        _browserFixture = browserFixture;
    }

    // InitializeAsync = 每個 [Fact] 執行前自動跑
    // 建立獨立的 BrowserContext，確保 cookie/localStorage/session 與其他 test 隔離
    // virtual 讓子 class 可以 override 加入共用的前置步驟（等同 NUnit 的 [SetUp]）
    public virtual async Task InitializeAsync()
    {
        _context = await _browserFixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 720 }
        });

        Page = await _context.NewPageAsync();
    }

    // DisposeAsync = 每個 [Fact] 執行後自動跑
    // 關閉 context 會同時清除這個 test 的所有 cookie/session，不影響下一個 test
    public async Task DisposeAsync()
    {
        if (_context != null) await _context.CloseAsync();
    }

    /// <summary>
    /// 處理網站的 Cookie 同意書對話框（Google Funding Choices）。
    /// 若對話框出現就點擊同意，若未出現則靜默略過。
    /// 每次導航到新頁面後應呼叫此方法。
    /// </summary>
    protected async Task DismissConsentDialogAsync()
    {
        try
        {
            await Page.Locator(".fc-cta-consent").ClickAsync(new LocatorClickOptions { Timeout = 5000 });
        }
        catch
        {
            // 同意書未出現，繼續執行
        }
    }

    /// <summary>
    /// 共用的登入 helper，讓需要登入的 test 可以直接呼叫。
    /// 透過 LoginPage 操作，遵守 POM 原則，selector 不重複定義。
    /// </summary>
    protected async Task LoginAsync(string email = Users.ValidEmail, string password = Users.ValidPassword)
    {
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateAsync();
        await DismissConsentDialogAsync();
        await loginPage.LoginWithAsync(email, password);
    }
}
