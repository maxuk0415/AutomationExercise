using Microsoft.Playwright;

namespace AutomationExercise.Tests.Base;

/// <summary>
/// 管理 Browser 的生命週期，整個 test class 只建立一次 browser。
///
/// 使用方式：在 test class 上加 [Collection("Browser")]，
/// 所有同一個 Collection 的 test class 共用同一個 browser instance。
///
/// 架構說明：
///   BrowserFixture（整個 Collection 共用，只啟動一次瀏覽器）
///       └── PlaywrightFixture（每個 [Fact] 建立新的 BrowserContext，達到 session 隔離）
///
/// 對應 NUnit 的概念：
///   BrowserFixture ≈ [OneTimeSetUp] at assembly level
///   PlaywrightFixture.InitializeAsync ≈ [SetUp]
/// </summary>
public class BrowserFixture : IAsyncLifetime
{
    private IPlaywright _playwright = null!;
    public IBrowser Browser { get; private set; } = null!;

    private string BrowserName =>
        Environment.GetEnvironmentVariable("BROWSER") ?? "chromium";

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();

        Browser = BrowserName.ToLower() switch
        {
            "firefox" => await _playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            }),
            "webkit" => await _playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            }),
            _ => await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            })
        };
    }

    public async Task DisposeAsync()
    {
        if (Browser != null) await Browser.CloseAsync();
        _playwright?.Dispose();
    }
}
