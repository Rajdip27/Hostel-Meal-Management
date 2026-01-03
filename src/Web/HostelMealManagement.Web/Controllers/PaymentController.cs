using HostelMealManagement.Application.Repositories;
using HostelMealManagement.Application.Repositories.SSLCommerz;
using HostelMealManagement.Application.ViewModel.SSLCommerz;
using HostelMealManagement.Infrastructure.Helper.Acls;
using Microsoft.AspNetCore.Mvc;
namespace HostelMealManagement.Web.Controllers;
public class PaymentController(ISSLCommerzService _ssl, IPaymentTransactionRepository paymentTransactionRepository, ISignInHelper signInHelper) : Controller
{
    public async Task<IActionResult> Pay(long BillId,long CycleId )
    {

        var paymentRequestViewModel = await paymentTransactionRepository.CreateSSLPaymentRequestViewModelAsync(
            memberId: signInHelper.UserId??1, // Replace with actual member id
            mealCycleId: CycleId,
            billId: BillId
        );

        if (paymentRequestViewModel is null)
        {
            TempData["Error"] = "Payment information not found or already paid.";
            return RedirectToAction("Index", "MealBill");
        }

        var fullHost = $"{Request.Scheme}://{Request.Host.Value}";
        var request = new SSLPaymentRequest(
            Amount: paymentRequestViewModel.Amount,
            TransactionId: Guid.NewGuid().ToString(),
            SuccessUrl: $"{fullHost}/payment/success",
            FailUrl: $"{fullHost}/payment/fail",
            CancelUrl: $"{fullHost}/payment/cancel",
            CustomerName: paymentRequestViewModel.CustomerName,
            CustomerEmail: paymentRequestViewModel.CustomerEmail,
            CustomerPhone: paymentRequestViewModel.CustomerPhone,
            ProductName: paymentRequestViewModel.ProductName
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
