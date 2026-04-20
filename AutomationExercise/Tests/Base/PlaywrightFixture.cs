using Microsoft.Playwright;
using AutomationExercise.Pages;

namespace AutomationExercise.Tests.Base;

/// <summary>
/// xUnit Playwright base class.
/// Implements IAsyncLifetime so xUnit automatically calls InitializeAsync / DisposeAsync before and after each [Fact].
///
/// Architecture:
///   - Browser layer: managed by BrowserFixture, launched once per test class (via IClassFixture)
///   - BrowserContext layer: each [Fact] creates an isolated context, ensuring cookies/sessions do not bleed between tests
///   - Page layer: each [Fact] has its own Page
///
/// Performance:
///   Old approach: launch a browser for each test (~1-3 seconds × N tests)
///   New approach: launch the browser once per class, create only a context per test (~50ms)
///
/// Usage:
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

    // InitializeAsync runs automatically before each [Fact]
    // Creates an isolated BrowserContext so cookies/localStorage/session are isolated from other tests
    // virtual allows subclasses to override and add shared setup steps (equivalent to NUnit's [SetUp])
    public virtual async Task InitializeAsync()
    {
        _context = await _browserFixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 720 }
        });

        Page = await _context.NewPageAsync();

        // After each page load, auto-hide ad overlay iframes to prevent them from intercepting clicks (especially noticeable on WebKit CI)
        // setInterval runs every second to catch dynamically injected ads as well
        await Page.AddInitScriptAsync(@"
            const hideAdOverlays = () => {
                document.querySelectorAll('iframe[id^=""aswift""]').forEach(el => {
                    el.style.pointerEvents = 'none';
                    el.style.display = 'none';
                });
                document.querySelectorAll('ins.adsbygoogle').forEach(el => {
                    el.style.pointerEvents = 'none';
                });
                document.querySelectorAll('div[id^=""google_ads""]').forEach(el => {
                    el.style.pointerEvents = 'none';
                });
            };
            if (document.readyState !== 'loading') {
                hideAdOverlays();
            } else {
                document.addEventListener('DOMContentLoaded', hideAdOverlays);
            }
            setInterval(hideAdOverlays, 500);
        ");
    }

    // DisposeAsync runs automatically after each [Fact]
    // Closing the context clears all cookies/session for this test, without affecting the next test
    public async Task DisposeAsync()
    {
        if (_context != null) await _context.CloseAsync();
    }

    /// <summary>
    /// Handles the site's cookie consent dialog (Google Funding Choices).
    /// Clicks accept if the dialog appears; silently skips if it does not.
    /// Should be called after each navigation to a new page.
    /// </summary>
    protected async Task DismissConsentDialogAsync()
    {
        try
        {
            await Page.Locator(".fc-cta-consent").ClickAsync(new LocatorClickOptions { Timeout = 5000 });
        }
        catch
        {
            // Consent dialog did not appear — continue
        }
    }

    /// <summary>
    /// Shared login helper for tests that require an authenticated session.
    /// Delegates to LoginPage to follow POM principles and avoid duplicating selectors.
    /// </summary>
    protected async Task LoginAsync(string email = Users.ValidEmail, string password = Users.ValidPassword)
    {
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateAsync();
        await DismissConsentDialogAsync();
        await loginPage.LoginWithAsync(email, password);
    }
}
