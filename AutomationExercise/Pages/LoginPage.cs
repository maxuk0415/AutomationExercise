using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// Handles the /login page: login form and signup entry point.
/// </summary>
public class LoginPage(IPage page)
{
    // --- Locators ---
    private ILocator LoginEmailInput    => page.Locator("input[data-qa='login-email']");
    private ILocator LoginPasswordInput => page.Locator("input[data-qa='login-password']");
    private ILocator LoginButton        => page.Locator("button[data-qa='login-button']");
    private ILocator LoginErrorMessage  => page.Locator("p:text('Your email or password is incorrect!')");

    private ILocator SignupNameInput    => page.Locator("input[data-qa='signup-name']");
    private ILocator SignupEmailInput   => page.Locator("input[data-qa='signup-email']");
    private ILocator SignupButton       => page.Locator("button[data-qa='signup-button']");
    private ILocator SignupErrorMessage => page.Locator("p:text('Email Address already exist!')");

    // --- Methods ---
    public async Task NavigateAsync()
        => await page.GotoAsync(Urls.Base + Urls.Login);

    public async Task LoginWithAsync(string email, string password)
    {
        await LoginEmailInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        await LoginEmailInput.FillAsync(email);
        await LoginPasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public async Task<string> GetLoginErrorMessageAsync()
        => await LoginErrorMessage.InnerTextAsync();

    public async Task EnterSignupDetailsAsync(string name, string email)
    {
        await SignupNameInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        await SignupNameInput.FillAsync(name);
        await SignupEmailInput.FillAsync(email);
        await SignupButton.ClickAsync();
    }

    public async Task<string> GetSignupErrorMessageAsync()
        => await SignupErrorMessage.InnerTextAsync();
}
