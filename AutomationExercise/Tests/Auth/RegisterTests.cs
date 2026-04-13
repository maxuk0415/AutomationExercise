using AutomationExercise.Pages;
using AutomationExercise.Tests.Base;

namespace AutomationExercise.Tests.Auth;

/// <summary>
/// 測試註冊功能：成功建立帳號、使用已存在 Email、空白欄位。
///
/// SetUp 策略：helper method
/// 原因：每個 test 需要不同的起始資料（不同 email、不同欄位組合），
/// 且成功註冊後需要刪除帳號避免資料累積。
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

        // Step 1：在 Login 頁面輸入 Signup 資料
        var loginPage = new LoginPage(Page);
        await loginPage.EnterSignupDetailsAsync(
            Registration.FirstName + " " + Registration.LastName,
            Registration.GenerateUniqueEmail());

        // Step 2：填寫詳細註冊表單
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

        // Step 3：驗證帳號建立成功
        Assert.Equal("ACCOUNT CREATED!", await registerPage.GetAccountCreatedMessageAsync());

        // Cleanup：刪除剛建立的帳號，避免測試資料累積
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
            email: Users.ValidEmail);  // 已存在的 email

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

        // 空白名字觸發 HTML5 native validation，URL 應保持在 /login
        Assert.Contains(Urls.Login, Page.Url);
    }
}
