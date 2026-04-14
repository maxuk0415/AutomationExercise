using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// 負責點擊 Signup 後跳轉的詳細帳號建立表單頁面。
/// </summary>
public class RegisterPage(IPage page)
{
    // --- Locators ---
    private ILocator TitleMrRadio      => page.Locator("input#id_gender1");
    private ILocator PasswordInput     => page.Locator("input#password");
    private ILocator DayDropdown       => page.Locator("select#days");
    private ILocator MonthDropdown     => page.Locator("select#months");
    private ILocator YearDropdown      => page.Locator("select#years");
    private ILocator FirstNameInput    => page.Locator("input#first_name");
    private ILocator LastNameInput     => page.Locator("input#last_name");
    private ILocator CompanyInput      => page.Locator("input#company");
    private ILocator Address1Input     => page.Locator("input#address1");
    private ILocator CountryDropdown   => page.Locator("select#country");
    private ILocator StateInput        => page.Locator("input#state");
    private ILocator CityInput         => page.Locator("input#city");
    private ILocator ZipcodeInput      => page.Locator("input#zipcode");
    private ILocator MobileInput       => page.Locator("input#mobile_number");
    private ILocator CreateAccountBtn  => page.Locator("button[data-qa='create-account']");
    private ILocator AccountCreatedMsg => page.Locator("h2[data-qa='account-created']");

    // --- Methods ---
    public async Task FillAccountDetailsAsync(
        string password,
        string firstName,
        string lastName,
        string address,
        string country,
        string state,
        string city,
        string zipcode,
        string mobile,
        string dobDay   = "10",
        string dobMonth = "5",
        string dobYear  = "1990")
    {
        // 等待表單第一個可互動元素，確保頁面已完全載入（Firefox 有時導航稍慢）
        await PasswordInput.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });
        await TitleMrRadio.ClickAsync();
        await PasswordInput.FillAsync(password);
        await DayDropdown.SelectOptionAsync(dobDay);
        await MonthDropdown.SelectOptionAsync(dobMonth);
        await YearDropdown.SelectOptionAsync(dobYear);
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await Address1Input.FillAsync(address);
        await CountryDropdown.SelectOptionAsync(country);
        await StateInput.FillAsync(state);
        await CityInput.FillAsync(city);
        await ZipcodeInput.FillAsync(zipcode);
        await MobileInput.FillAsync(mobile);
    }

    public async Task SubmitRegistrationAsync()
        => await CreateAccountBtn.ClickAsync();

    public async Task<string> GetAccountCreatedMessageAsync()
    {
        // 60s timeout：CI 環境下網站後端有時偏慢，30s 不夠用（Firefox 觀察到 2 次連續 timeout）
        await AccountCreatedMsg.WaitForAsync(new LocatorWaitForOptions { Timeout = 60000 });
        return await AccountCreatedMsg.InnerTextAsync();
    }
}
