using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// Handles the /products page: search, filter, and add to cart.
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

    // --- Dynamic Locators (dynamic values returned as ILocator methods, not inlined) ---
    // Category title link (e.g. "Women", "Men") — id corresponds to the expanded panel
    private ILocator CategoryLink(string name)    => page.Locator($".panel-title a:has-text('{name}')");
    // Sub-category link: scoped inside the parent category panel to avoid cross-category mismatches (e.g. Women/Dress vs Kids/Dress)
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
        // WebKit: the page needs time to update after searching — wait for DOM to settle before reading results
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
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
        await AddToCartButtons.First.ScrollIntoViewIfNeededAsync();
        // Force = true: the Add to Cart button inside .productinfo is only visible on CSS hover (WebKit cannot trigger hover automatically)
        // WaitForAsync(Visible) will time out in WebKit; Force bypasses visibility and clicks the DOM element directly
        await AddToCartButtons.First.ClickAsync(new LocatorClickOptions { Force = true });
        await ContinueShoppingBtn.ClickAsync();
    }

    public async Task FilterByCategoryAsync(string category, string subCategory)
    {
        var catLink = CategoryLink(category);
        await catLink.ScrollIntoViewIfNeededAsync();
        await catLink.ClickAsync();
        var subCatLink = SubCategoryLink(category, subCategory);
        // Wait for element to be in the DOM (WebKit accordion animation keeps it CSS-hidden, so Visible will never resolve)
        await subCatLink.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Attached, Timeout = 10000 });
        // JS native click bypasses CSS visibility restriction (WebKit accordion)
        await subCatLink.EvaluateAsync("el => el.click()");
        // WaitForURLAsync: wait for URL to change to /category_products/...
        // More reliable than WaitForLoadStateAsync: won't immediately resolve if the current page is already loaded (Chromium race condition)
        await page.WaitForURLAsync("**/category_products/**",
            new PageWaitForURLOptions { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 10000 });
    }

    public async Task FilterByBrandAsync(string brandName)
        => await BrandLink(brandName).ClickAsync();

    public async Task ClickFirstProductAsync()
    {
        // .choose div may be display:none in WebKit (no layout box)
        // Force=true won't work: it would click at coordinates (0,0) instead of the link itself
        // Solution: get the href attribute from the DOM and navigate directly with GotoAsync, bypassing CSS visibility entirely
        var href = await ViewProductLinks.First.EvaluateAsync<string>("el => el.getAttribute('href')");
        await page.GotoAsync(Urls.Base + href,
            new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
    }
}
