# AutomationExercise E2E Test Automation

Portfolio project — Cross-browser E2E automation for [automationexercise.com](https://automationexercise.com)

## Tech Stack

| Tool | Version | Purpose |
|------|---------|---------|
| C# / .NET | 10 | Primary language |
| xUnit | 2.9.3 | Test framework |
| Microsoft.Playwright | 1.59.0 | Browser automation |
| GitHub Actions | — | CI/CD (Chromium / Firefox / WebKit) |

## Project Structure

```
AutomationExercise/
├── Pages/                    # Page Object Model
│   ├── NavBarPage.cs
│   ├── LoginPage.cs
│   ├── RegisterPage.cs
│   ├── ProductsPage.cs
│   ├── ProductDetailPage.cs
│   ├── CartPage.cs
│   ├── CheckoutPage.cs
│   ├── PaymentPage.cs
│   ├── PaymentDonePage.cs
│   ├── ContactUsPage.cs
│   └── AccountDeletedPage.cs
├── Tests/
│   ├── Base/                 # BrowserFixture, PlaywrightFixture, ApiFixture
│   ├── Auth/                 # Login, Register
│   ├── Products/             # Search, Filter
│   ├── Cart/                 # Cart management
│   ├── Checkout/             # Order flow
│   └── ContactUs/            # Contact form
├── TestAssets/               # Upload test files
├── TestData.cs               # Centralised test constants
└── xunit.runner.json         # Parallel config
```

## Test Coverage

| Test File | Count | Coverage |
|-----------|-------|---------|
| LoginTests | 4 | Login success, unregistered email, wrong password, empty fields |
| RegisterTests | 3 | Account created, existing email, empty name |
| ProductsTests | 6 | Browse, search (valid/invalid), brand filter, category filter, product detail |
| CartTests | 4 | Add to cart, product info, delete item, guest checkout modal |
| CheckoutTests | 3 | Navigate to checkout, order summary, successful payment |
| ContactUsTests | 2 | Submit form with upload, Home navigation after submit |
| ApiTests | 8 | productsList GET/POST, brandsList GET/PUT, searchProduct (valid/missing), verifyLogin (valid/invalid) |
| **Total** | **30** | |

## Running Tests

```bash
# All tests (Chromium by default)
dotnet test

# Specific browser
BROWSER=firefox dotnet test
BROWSER=webkit  dotnet test

# Specific test class
dotnet test --filter "FullyQualifiedName~Tests.Auth"
```

## CI/CD

GitHub Actions runs all 30 tests on **Chromium**, **Firefox**, and **WebKit** in parallel on every push.

See `.github/workflows/playwright.yml`.

## Architecture Decisions

- **BrowserFixture** — shared per test class (`IClassFixture<BrowserFixture>`), browser launched once
- **PlaywrightFixture** — new `BrowserContext` per `[Fact]`, ensures session isolation
- **xUnit `IAsyncLifetime`** — `InitializeAsync` = SetUp, `DisposeAsync` = TearDown
- **`virtual InitializeAsync`** — subclasses can override for shared navigation setup
- **`WaitForAsync` over `IsVisibleAsync`** — all assertions wait for elements, never snapshot-only
- **Dynamic locators** — private `ILocator` methods, not inline strings scattered in tests
