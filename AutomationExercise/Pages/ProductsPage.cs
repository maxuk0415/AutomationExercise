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
        var catLink = CategoryLink(category);
        await catLink.ScrollIntoViewIfNeededAsync();
        await catLink.ClickAsync();
        var subCatLink = SubCategoryLink(category, subCategory);
        // 等元素進入 DOM（WebKit accordion 動畫期間元素是 CSS hidden，Visible 永遠等不到）
        await subCatLink.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Attached, Timeout = 10000 });
        // 用 JS native click：直接呼叫元素的 .click()，繞過 CSS visibility 限制
        // 比 Force=true 更安全：觸發正確的 href 導航，不模擬滑鼠座標
        await subCatLink.EvaluateAsync("el => el.click()");
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task FilterByBrandAsync(string brandName)
        => await BrandLink(brandName).ClickAsync();

    public async Task ClickFirstProductAsync()
    {
        await ViewProductLinks.First.ScrollIntoViewIfNeededAsync();
        await ViewProductLinks.First.ClickAsync();
    }
}
