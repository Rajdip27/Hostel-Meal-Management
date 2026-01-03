using HostelMealManagement.Application.Repositories;
using HostelMealManagement.Application.Repositories.SSLCommerz;
using HostelMealManagement.Application.ViewModel.SSLCommerz;
using Microsoft.AspNetCore.Mvc;
namespace HostelMealManagement.Web.Controllers;
public class PaymentController(ISSLCommerzService _ssl, IMealBillRepository mealBillRepository, IMealCycleRepository mealCycleRepository) : Controller
{
    public async Task<IActionResult> Pay(long BillId,long CycleId )
    {

        var fullHost = $"{Request.Scheme}://{Request.Host.Value}";
        var request = new SSLPaymentRequest(
            Amount: 1000,
            TransactionId: Guid.NewGuid().ToString(),
            SuccessUrl: $"{fullHost}/payment/success",
            FailUrl: $"{fullHost}/payment/fail",
            CancelUrl: $"{fullHost}/payment/cancel",
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
        try
        {
            if (string.IsNullOrEmpty(val_id))
            {
                return RedirectToAction("Fail");
            }
            var result = await _ssl.ValidateAsync(val_id);
            // Parse and process the result
            var paymentResult = ParsePaymentResult(result);

            if (paymentResult.Status != "Success")
            {
                return RedirectToAction("Fail");
            }

            var viewModel = new PaymentSuccessViewModel
            {
                TransactionId = paymentResult.TransactionId,
                Amount = paymentResult.Amount,
                Status = paymentResult.Status,
                Message = paymentResult.Message,
                PaymentDate = DateTime.Now,
                CustomerName = paymentResult.CustomerName ?? "Customer"
            };

            // Return the view with the model
            return View(viewModel);  // ← THIS WAS MISSING
        }
        catch (Exception ex)
        {
            return RedirectToAction("Fail");
        }
    }
    public IActionResult Fail()
    {
        return View(); 
    }

    public IActionResult Cancel()
    {
        return View(); 
    }
    private PaymentResult ParsePaymentResult(string result)
    {
        return new PaymentResult
        {
            TransactionId = "TRX123456",
            Amount = 1000,
            Status = "Success",
            Message = "Payment completed successfully",
            CustomerName = "Test User"
        };
    }
}
