using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// 負責 /checkout 頁面：確認地址、查看訂單摘要、輸入備註、送出訂單。
/// </summary>
public class CheckoutPage(IPage page)
{
    // --- Locators ---
    private ILocator DeliveryAddress => page.Locator("#address_delivery");
    private ILocator BillingAddress  => page.Locator("#address_invoice");
    private ILocator OrderItems      => page.Locator("#cart_info tbody tr");
    private ILocator CommentTextArea => page.Locator("textarea.form-control");
    private ILocator PlaceOrderBtn   => page.Locator("a.btn.btn-default.check_out");

    // --- Methods ---
    public async Task<string> GetDeliveryFullNameAsync()
        => await DeliveryAddress.Locator(".address_firstname.address_lastname").InnerTextAsync();

    public async Task<int> GetOrderItemCountAsync()
        => await OrderItems.CountAsync();

    public async Task<string> GetOrderItemNameAsync(int rowIndex = 0)
        => await OrderItems.Nth(rowIndex).Locator("td.cart_description h4 a").InnerTextAsync();

    public async Task EnterCommentAsync(string comment)
        => await CommentTextArea.FillAsync(comment);

    public async Task PlaceOrderAsync()
    {
        await PlaceOrderBtn.ScrollIntoViewIfNeededAsync();
        await PlaceOrderBtn.ClickAsync();
        // 等待導航至付款頁面；若不等待，FillPaymentDetailsAsync 會在舊頁面找不到 input → 30s timeout
        await page.WaitForURLAsync("**/payment",
            new PageWaitForURLOptions { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 30000 });
    }
}
