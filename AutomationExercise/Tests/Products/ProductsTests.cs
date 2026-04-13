using AutomationExercise.Pages;
using AutomationExercise.Tests.Base;
using ProductData = AutomationExercise.Products; // alias 避免與 Tests.Products namespace 衝突

namespace AutomationExercise.Tests.Products;

/// <summary>
/// 測試產品瀏覽功能：顯示商品、商品詳情、分類篩選、品牌篩選、搜尋。
///
/// SetUp 策略：override InitializeAsync（等同 NUnit 的 [SetUp]）
/// 原因：所有測試都從 /products 開始，統一導航，避免每個 test 重複寫。
///
/// 注意：_productsPage 不能在 constructor 初始化，因為 Page 在 base.InitializeAsync()
/// 之後才會被建立。必須在 override InitializeAsync 裡，呼叫 base 之後才能初始化。
/// </summary>
public class ProductsTests : PlaywrightFixture, IClassFixture<BrowserFixture>
{
    private ProductsPage _productsPage = null!;

    public ProductsTests(BrowserFixture browser) : base(browser) { }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();             // 先讓 base 建立 Page
        _productsPage = new ProductsPage(Page);   // Page 現在已可用
        await _productsPage.NavigateAsync();
        await DismissConsentDialogAsync();
    }

    [Fact]
    public async Task ShouldDisplayProductsWhenNavigatingToProductsPage()
    {
        var count = await _productsPage.GetProductCountAsync();
        Assert.True(count > 0);
    }

    [Fact]
    public async Task ShouldShowProductDetailWhenClickingOnProduct()
    {
        await _productsPage.ClickFirstProductAsync();

        var detailPage = new ProductDetailPage(Page);
        var productName = await detailPage.GetProductNameAsync();
        Assert.False(string.IsNullOrEmpty(productName));
    }

    [Fact]
    public async Task ShouldShowSearchResultsWhenSearchingWithValidKeyword()
    {
        await _productsPage.SearchProductAsync(ProductData.SearchKeyword);

        var count = await _productsPage.GetProductCountAsync();
        Assert.True(count > 0);
    }

    [Fact]
    public async Task ShouldShowZeroResultsWhenSearchingWithInvalidKeyword()
    {
        await _productsPage.SearchProductAsync(ProductData.NoResultKeyword);

        var count = await _productsPage.GetProductCountAsync();
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task ShouldFilterProductsWhenSelectingWomenDressCategory()
    {
        await _productsPage.FilterByCategoryAsync(ProductData.CategoryWomen, ProductData.SubCategoryDress);

        var count = await _productsPage.GetProductCountAsync();
        Assert.True(count > 0);
    }

    [Fact]
    public async Task ShouldFilterProductsWhenClickingHAndMBrand()
    {
        await _productsPage.FilterByBrandAsync(ProductData.BrandHAndM);

        var count = await _productsPage.GetProductCountAsync();
        Assert.True(count > 0);
    }
}
