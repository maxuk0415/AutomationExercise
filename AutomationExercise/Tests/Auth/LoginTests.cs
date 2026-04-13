using AutomationExercise.Pages;
using AutomationExercise.Tests.Base;

namespace AutomationExercise.Tests.Auth;

public class LoginTests : PlaywrightFixture, IClassFixture<BrowserFixture>
{
    public LoginTests(BrowserFixture browser) : base(browser) { }

    [Fact]
    public async Task ShouldShowLoggedInUsernameWhenLoginWithValidCredentials()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateAsync();
        await DismissConsentDialogAsync();
        await loginPage.LoginWithAsync(Users.ValidEmail, Users.ValidPassword);

        var navBar = new NavBarPage(Page);
        Assert.True(await navBar.IsLoggedInAsync());
        Assert.Equal(Users.ValidName, await navBar.GetLoggedInUsernameAsync());
    }

    [Fact]
    public async Task ShouldShowErrorWhenLoginWithWrongPassword()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateAsync();
        await DismissConsentDialogAsync();
        await loginPage.LoginWithAsync(Users.ValidEmail, Users.WrongPassword);

        Assert.Equal(
            "Your email or password is incorrect!",
            await loginPage.GetLoginErrorMessageAsync());
    }

    [Fact]
    public async Task ShouldShowErrorWhenLoginWithUnregisteredEmail()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateAsync();
        await DismissConsentDialogAsync();
        await loginPage.LoginWithAsync(Users.WrongEmail, Users.ValidPassword);

        Assert.Equal(
            "Your email or password is incorrect!",
            await loginPage.GetLoginErrorMessageAsync());
    }

    [Fact]
    public async Task ShouldStayOnLoginPageWhenSubmitWithEmptyFields()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateAsync();
        await DismissConsentDialogAsync();
        await loginPage.LoginWithAsync(string.Empty, string.Empty);

        Assert.Contains(Urls.Login, Page.Url);
    }
}
