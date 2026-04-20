using Microsoft.Playwright;

namespace AutomationExercise.Pages;

/// <summary>
/// Handles the /checkout page: confirm address, view order summary, enter comments, place order.
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
        // Not using WaitForURLAsync: the payment page URL format is inconsistent (may include query strings or different paths)
        // Instead, the WaitForAsync inside FillPaymentDetailsAsync ensures the payment form has appeared
    }
}
