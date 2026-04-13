using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// 負責 /products 頁面：搜尋、篩選、加入購物車。
/// </summary>
public class ProductsPage(IPage page)
{
    // --- Static Locators ---
    private ILocator SearchInput         => page.Locator("input#search_product");
    private ILocator SearchButton        => page.Locator("button#submit_search");
    private ILocator ProductCards        => page.Locator(".productinfo");
    private ILocator SearchResultTitle   => page.Locator("h2.title.text-center");
    private ILocator AddToCartButtons    => page.Locator(".productinfo a.btn");
    private ILocator ContinueShoppingBtn => page.Locator("button:text('Continue Shopping')");
    private ILocator ViewCartLink        => page.Locator("u:text('View Cart')");
    private ILocator ViewProductLinks    => page.Locator(".choose a:text('View Product')");

    // --- Dynamic Locators（動態值用方法回傳 ILocator，不散落在方法裡）---
    // 分類標題連結（如 "Women"、"Men"），id 對應展開的 panel
    private ILocator CategoryLink(string name)    => page.Locator($".panel-title a:has-text('{name}')");
    // 子分類連結：限定在 parent category panel 內，避免跨分類誤選（如 Women/Dress vs Kids/Dress）
    private ILocator SubCategoryLink(string category, string subCategory)
        => page.Locator($"#{category} a:has-text('{subCategory}')");
    private ILocator BrandLink(string brand)      => page.Locator($".brands-name a:has-text('{brand}')");

    // --- Methods ---
    public async Task NavigateAsync()
        => await page.GotoAsync(Urls.Base + Urls.Products);

    public async Task SearchProductAsync(string keyword)
    {
        await SearchInput.FillAsync(keyword);
        await SearchButton.ClickAsync();
    }

    public async Task<int> GetProductCountAsync()
        => await ProductCards.CountAsync();

    public async Task<List<string>> GetAllProductNamesAsync()
    {
        var names = new List<string>();
        var count = await ProductCards.CountAsync();
        for (int i = 0; i < count; i++)
        {
            var name = await ProductCards.Nth(i).Locator("p").InnerTextAsync();
            names.Add(name);
        }
        return names;
    }

    public async Task<string> GetSearchResultTitleAsync()
        => await SearchResultTitle.InnerTextAsync();

    public async Task AddFirstProductToCartAsync()
    {
        await AddToCartButtons.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        await AddToCartButtons.First.ClickAsync();
        await ContinueShoppingBtn.ClickAsync();
    }

    public async Task FilterByCategoryAsync(string category, string subCategory)
    {
        // 先 scroll 到分類標題，確保在 WebKit 的視窗內再點擊
        var catLink = CategoryLink(category);
        await catLink.ScrollIntoViewIfNeededAsync();
        await catLink.ClickAsync();
        // 等待子分類可見（accordion 展開動畫完成），加大 timeout 以兼容 WebKit 較慢的動畫
        var subCatLink = SubCategoryLink(category, subCategory);
        await subCatLink.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 60000 });
        await subCatLink.ScrollIntoViewIfNeededAsync();
        await subCatLink.ClickAsync();
    }

    public async Task FilterByBrandAsync(string brandName)
        => await BrandLink(brandName).ClickAsync();

    public async Task ClickFirstProductAsync()
    {
        await ViewProductLinks.First.ScrollIntoViewIfNeededAsync();
        await ViewProductLinks.First.ClickAsync();
    }
}
