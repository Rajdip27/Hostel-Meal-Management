using HostelMealManagement.Application.Repositories.SSLCommerz;
using HostelMealManagement.Application.ViewModel.SSLCommerz;
using Microsoft.AspNetCore.Mvc;

namespace HostelMealManagement.Web.Controllers;

public class PaymentController(ISSLCommerzService _ssl) : Controller
{
    public async Task<IActionResult> Pay()
    {
        var request = new SSLPaymentRequest(
            Amount: 1000,
            TransactionId: Guid.NewGuid().ToString(),
            SuccessUrl: "https://localhost:7087/payment/success",
            FailUrl: "https://localhost:7087/payment/fail",
            CancelUrl: "https://localhost:7087/payment/cancel",
            CustomerName: "Test User",
            CustomerEmail: "test@piistech.com",
            CustomerPhone: "01700000000",
            ProductName: "Software Service"
        );

        var url = await _ssl.CreatePaymentAsync(request);
        return Redirect(url);
    }

    public async Task<IActionResult> Success(string val_id)
    {
        var result = await _ssl.ValidateAsync(val_id);
        return Content(result);
    }
}
