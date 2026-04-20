using AutomationExercise.Pages;
using AutomationExercise.Tests.Base;
using ProductData = AutomationExercise.Products; // alias to avoid conflict with Tests.Products namespace

namespace AutomationExercise.Tests.Products;

/// <summary>
/// Tests product browsing: displaying products, product detail, category filtering, brand filtering, search.
///
/// SetUp strategy: override InitializeAsync (equivalent to NUnit's [SetUp])
/// Reason: all tests start from /products, so navigation is centralised to avoid repeating it in each test.
///
/// Note: _productsPage cannot be initialised in the constructor because Page is only available
/// after base.InitializeAsync() completes. It must be initialised in override InitializeAsync, after calling base.
/// </summary>
public class ProductsTests : PlaywrightFixture, IClassFixture<BrowserFixture>
{
    private ProductsPage _productsPage = null!;

    public ProductsTests(BrowserFixture browser) : base(browser) { }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();             // let base create the Page first
        _productsPage = new ProductsPage(Page);   // Page is now available
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
        await DismissConsentDialogAsync(); // consent may reappear on the product detail page

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
