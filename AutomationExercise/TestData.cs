namespace AutomationExercise;

public static class Urls
{
    public const string Base           = "https://automationexercise.com";
    public const string Login          = "/login";
    public const string Products       = "/products";
    public const string Cart           = "/view_cart";
    public const string Checkout       = "/checkout";
    public const string ContactUs      = "/contact_us";
    public const string Account        = "/account";
    public const string AccountDeleted = "/delete_account";
}

public static class Users
{
    // Pre-created account on the site (fixed account — not recreated on each run)
    public const string ValidEmail    = "max_autotest@example.com";
    public const string ValidPassword = "Test1234!";
    public const string ValidName     = "Max AutoTest";

    // Used for testing the "account does not exist" scenario
    public const string WrongEmail    = "notexist_xyz@example.com";
    public const string WrongPassword = "WrongPass999!";
}

public static class Registration
{
    public const string FirstName   = "Max";
    public const string LastName    = "Tester";
    public const string Company     = "Test Corp";
    public const string Address     = "123 Test Street";
    public const string Country     = "United States";
    public const string State       = "California";
    public const string City        = "Los Angeles";
    public const string Zipcode     = "90001";
    public const string MobilePhone = "0987654321";
    public const string Password    = "Register1234!";

    // Generates a unique email each call to avoid "Email Address already exist!" errors
    // Each call produces a different value, e.g. autotest_20260412153045@example.com
    public static string GenerateUniqueEmail()
        => $"autotest_{DateTime.UtcNow:yyyyMMddHHmmss}@example.com";
}

public static class Products
{
    public const string SearchKeyword    = "Dress";
    public const string NoResultKeyword  = "xyznonexistent999";
    public const string CategoryWomen    = "Women";
    public const string SubCategoryDress = "Dress";
    public const string BrandHAndM       = "H&M";

    // Known name of the first search result for keyword "Dress" — used for cart verification
    // Avoids the low-information assertion of validating a retrieved value against itself
    public const string FirstSearchResult = "Fancy Green Top";
}

public static class PaymentData
{
    public const string CardName    = "Max AutoTest";
    public const string CardNumber  = "4111111111111111";
    public const string Cvc         = "123";
    public const string ExpiryMonth = "12";
    public const string ExpiryYear  = "2027";
}

public static class ContactForm
{
    public const string Name           = "Max AutoTest";
    public const string Email          = "max_autotest@example.com";
    public const string Subject        = "Test Inquiry";
    public const string Message        = "This is an automated test message. Please ignore.";
    public const string UploadFilePath = "TestAssets/test-upload.txt";
}
