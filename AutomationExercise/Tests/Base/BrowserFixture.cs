using Microsoft.Playwright;

namespace AutomationExercise.Tests.Base;

/// <summary>
/// Manages the Browser lifecycle, launching it only once per test class.
///
/// Usage: add [Collection("Browser")] to the test class.
/// All test classes in the same Collection share the same browser instance.
///
/// Architecture:
///   BrowserFixture (shared across the entire Collection — browser launched only once)
///       └── PlaywrightFixture (each [Fact] creates a new BrowserContext for session isolation)
///
/// NUnit equivalents:
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
