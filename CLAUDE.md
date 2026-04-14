# AutomationExercise — E2E Test Automation Portfolio

## 專案概覽
- **目標網站**：https://automationexercise.com
- **目的**：求職 portfolio，展示 E2E test automation framework 架構能力
- **Tech Stack**：C# / xUnit / Playwright / Page Object Model / GitHub Actions

## 專案路徑
```
~/Documents/MyPlace/SoftwareLearning/Playwright/AutomationExercise/
├── .github/workflows/playwright.yml   ← 三瀏覽器 CI/CD
└── AutomationExercise/
    ├── Pages/                          ← 11 個 Page Objects
    ├── Tests/
    │   ├── Base/                       ← BrowserFixture, PlaywrightFixture, ApiFixture
    │   ├── Auth/                       ← Login, Register 測試
    │   ├── Products/                   ← 搜尋、篩選測試
    │   ├── Cart/                       ← 購物車測試
    │   ├── Checkout/                   ← 結帳流程測試
    │   ├── ContactUs/                  ← 聯絡表單測試
    │   └── Api/                        ← API 測試（APIRequestContext）
    ├── TestAssets/test-upload.txt      ← ContactUs 上傳測試用
    ├── TestData.cs                     ← 所有常數集中管理
    └── xunit.runner.json               ← 平行化設定
```

## xUnit Fixture 架構
```
BrowserFixture          → 整個 test class 共用一個 browser（IClassFixture<BrowserFixture>）
  └── PlaywrightFixture → 每個 [Fact] 建立獨立 BrowserContext（session 隔離）
ApiFixture              → API 測試專用，不啟動 browser（直接繼承）
```

**test class 使用方式：**
```csharp
[Collection("Browser")]
public class LoginTests : PlaywrightFixture
{
    public LoginTests(BrowserFixture browser) : base(browser) { }
}
```

## 測試命名規範
`Should[預期行為]When[條件]`

範例：
- `ShouldRedirectToDashboardWhenLoginWithValidCredentials`
- `ShouldShowErrorWhenLoginWithWrongPassword`

## 重要設計決策

### 使用 xUnit 而非 NUnit
公司使用 xUnit，portfolio 與工作保持一致。Playwright 的 Browser/Context lifecycle 透過自訂 `BrowserFixture` + `PlaywrightFixture` 管理。

### API 測試策略
automationexercise.com 的 API 永遠回傳 HTTP 200，真正的錯誤在 JSON body 的 `responseCode` 欄位。
斷言時必須同時驗證兩個值，不能只驗證 HTTP status code。

### Dynamic Email（Registration 測試）
`Registration.GenerateUniqueEmail()` 每次產生不同 email，避免「Email already exist」錯誤。

## 目前的測試範圍
- `LoginTests.cs` — 登入測試（正向 1 個、負向 3 個，共 **4 個**）
- `RegisterTests.cs` — 註冊測試（正向 1 個、負向 2 個，共 **3 個**）
- `ProductsTests.cs` — 商品搜尋 / 篩選測試（共 **6 個**：瀏覽、搜尋有效/無效關鍵字、品牌篩選、類別篩選、商品詳情）
- `CartTests.cs` — 購物車測試（共 **4 個**：加入、商品資訊確認、刪除、Guest Checkout 彈窗）
- `CheckoutTests.cs` — 結帳流程測試（共 **3 個**：導航、訂單摘要、成功下單）
- `ContactUsTests.cs` — 聯絡表單測試（共 **2 個**：成功送出含上傳、點擊 Home 導回首頁）
- `ApiTests.cs` — API 測試（共 **8 個**：productsList GET/POST、brandsList GET/PUT、searchProduct 有效/無效、verifyLogin 有效/無效）

**總計：30 個測試 ✅**

## 測試覆蓋概覽
| Test File | 測試數 | 涵蓋功能 |
|-----------|--------|---------|
| LoginTests | 4 | 登入成功、帳號不存在、密碼錯誤、空欄位 |
| RegisterTests | 3 | 帳號建立、Email 已存在、空白姓名 |
| ProductsTests | 6 | 瀏覽、有效/無效搜尋、品牌篩選、類別篩選、商品詳情 |
| CartTests | 4 | 加入、資訊確認、刪除、Guest Checkout 彈窗 |
| CheckoutTests | 3 | 進入結帳、訂單摘要、付款成功 |
| ContactUsTests | 2 | 表單送出（含上傳）、Home 導覽 |
| ApiTests | 8 | productsList、brandsList、searchProduct、verifyLogin |
| **Total** | **30** | |

## CI/CD 策略
- **每次 push/PR**：Chromium、Firefox 失敗則 block；WebKit 使用 `continue-on-error`（結果可見但不 block pipeline）
- **夜間排程（UTC 00:00，台灣 08:00）**：三瀏覽器完整執行，監控 WebKit 長期穩定性
- **WebKit non-blocking 原因**：automationexercise.com 有廣告和 consent dialog，在 GitHub Actions 共享 runner 上行為不穩定，屬於基礎設施變因而非測試或應用程式 bug

## 不測的項目（有意識排除）
- API Testing 文件頁（無 UI 互動）
- YouTube 連結（第三方）
- 真實金融交易（使用假卡號）
- RWD 細節（需專用視覺測試工具）
- 效能測試（超出 E2E 範疇）
