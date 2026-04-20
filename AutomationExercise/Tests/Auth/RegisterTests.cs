using AutomationExercise.Pages;
using AutomationExercise.Tests.Base;

namespace AutomationExercise.Tests.Auth;

/// <summary>
/// Tests registration: successfully creating an account, using an existing email, empty fields.
///
/// SetUp strategy: helper method
/// Reason: each test requires different starting data (different email, different field combinations),
/// and a successfully registered account must be deleted afterwards to avoid data accumulation.
/// </summary>
public class RegisterTests : PlaywrightFixture, IClassFixture<BrowserFixture>
{
    public RegisterTests(BrowserFixture browser) : base(browser) { }

    private async Task NavigateToSignupAsync()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateAsync();
        await DismissConsentDialogAsync();
        return;
    }

    [Fact]
    public async Task ShouldShowAccountCreatedWhenRegisterWithValidDetails()
    {
        await NavigateToSignupAsync();

        // Step 1: enter Signup details on the Login page
        var loginPage = new LoginPage(Page);
        await loginPage.EnterSignupDetailsAsync(
            Registration.FirstName + " " + Registration.LastName,
            Registration.GenerateUniqueEmail());

        // Step 2: fill in the detailed registration form
        var registerPage = new RegisterPage(Page);
        await registerPage.FillAccountDetailsAsync(
            password:  Registration.Password,
            firstName: Registration.FirstName,
            lastName:  Registration.LastName,
            address:   Registration.Address,
            country:   Registration.Country,
            state:     Registration.State,
            city:      Registration.City,
            zipcode:   Registration.Zipcode,
            mobile:    Registration.MobilePhone);
        await registerPage.SubmitRegistrationAsync();

        // Step 3: verify account was created successfully
        Assert.Equal("ACCOUNT CREATED!", await registerPage.GetAccountCreatedMessageAsync());

        // Cleanup: delete the newly created account to prevent test data accumulation
        await Page.ClickAsync("a[data-qa='continue-button']");
        var accountPage = new AccountPage(Page);
        await accountPage.DeleteAccountAsync();
    }

    [Fact]
    public async Task ShouldShowErrorWhenRegisterWithExistingEmail()
    {
        await NavigateToSignupAsync();

        var loginPage = new LoginPage(Page);
        await loginPage.EnterSignupDetailsAsync(
            name:  Users.ValidName,
            email: Users.ValidEmail);  // existing email

        Assert.Equal(
            "Email Address already exist!",
            await loginPage.GetSignupErrorMessageAsync());
    }

    [Fact]
    public async Task ShouldStayOnLoginPageWhenSignupWithEmptyName()
    {
        await NavigateToSignupAsync();

        var loginPage = new LoginPage(Page);
        await loginPage.EnterSignupDetailsAsync(name: string.Empty, email: "test@example.com");

        // An empty name triggers HTML5 native validation — URL should remain at /login
        Assert.Contains(Urls.Login, Page.Url);
    }
}
