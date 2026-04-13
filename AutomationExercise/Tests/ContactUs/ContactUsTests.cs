using AutomationExercise.Pages;
using AutomationExercise.Tests.Base;

namespace AutomationExercise.Tests.ContactUs;

/// <summary>
/// 測試聯絡表單功能：送出表單、導回首頁。
///
/// SetUp 策略：override InitializeAsync
/// 原因：所有測試都從 /contact_us 開始，統一導航。
/// </summary>
public class ContactUsTests : PlaywrightFixture, IClassFixture<BrowserFixture>
{
    private ContactUsPage _contactUsPage = null!;

    public ContactUsTests(BrowserFixture browser) : base(browser) { }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _contactUsPage = new ContactUsPage(Page);
        await _contactUsPage.NavigateAsync();
        await DismissConsentDialogAsync();
    }

    [Fact]
    public async Task ShouldShowSuccessMessageWhenSubmitContactFormWithValidData()
    {
        await _contactUsPage.FillContactFormAsync(
            name:    ContactForm.Name,
            email:   ContactForm.Email,
            subject: ContactForm.Subject,
            message: ContactForm.Message);
        await _contactUsPage.UploadFileAsync(ContactForm.UploadFilePath);
        await _contactUsPage.SubmitFormAsync();

        Assert.True(await _contactUsPage.IsSuccessMessageVisibleAsync());
    }

    [Fact]
    public async Task ShouldNavigateToHomePageWhenClickingHomeAfterSubmit()
    {
        await _contactUsPage.FillContactFormAsync(
            name:    ContactForm.Name,
            email:   ContactForm.Email,
            subject: ContactForm.Subject,
            message: ContactForm.Message);
        await _contactUsPage.SubmitFormAsync();

        await _contactUsPage.ClickHomeAsync();

        Assert.Contains(Urls.Base, Page.Url);
        Assert.DoesNotContain(Urls.ContactUs, Page.Url);
    }
}
