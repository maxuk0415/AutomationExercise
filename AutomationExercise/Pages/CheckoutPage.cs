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
        // 不用 WaitForURLAsync：付款頁面 URL 格式不固定（可能含 query string 或不同 path）
        // 改由 FillPaymentDetailsAsync 內的 WaitForAsync 確保付款表單已出現
    }
}
