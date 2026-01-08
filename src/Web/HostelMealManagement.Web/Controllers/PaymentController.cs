using HostelMealManagement.Application.CommonModel;
using HostelMealManagement.Application.Extensions;
using HostelMealManagement.Application.Repositories;
using HostelMealManagement.Application.Repositories.SSLCommerz;
using HostelMealManagement.Application.ViewModel.SSLCommerz;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.Helper.Acls;
using HostelMealManagement.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static HostelMealManagement.Core.Entities.Auth.IdentityModel;

namespace HostelMealManagement.Web.Controllers;

public class PaymentController : Controller
{
    private readonly ISSLCommerzService _ssl;
    private readonly IPaymentTransactionRepository _paymentTransactionRepository;
    private readonly ISignInHelper _signInHelper;
    private readonly IMealBillRepository _mealBillRepository;

    // ✅ ADDED (EMAIL SUPPORT)
    private readonly IEmailService _emailService;
    private readonly UserManager<User> _userManager;

    public PaymentController(
        ISSLCommerzService ssl,
        IPaymentTransactionRepository paymentTransactionRepository,
        ISignInHelper signInHelper,
        IMealBillRepository mealBillRepository,
        IEmailService emailService,            // ✅ ADD
        UserManager<User> userManager)         // ✅ ADD
    {
        _ssl = ssl;
        _paymentTransactionRepository = paymentTransactionRepository;
        _signInHelper = signInHelper;
        _mealBillRepository = mealBillRepository;
        _emailService = emailService;          // ✅ ADD
        _userManager = userManager;            // ✅ ADD
    }

    // ===========================
    // PAYMENT INIT
    // ===========================
    [HttpGet]
    public async Task<IActionResult> Pay(long billId, long cycleId)
    {
        var memberId = _signInHelper.UserId ?? 1;

        var paymentVM =
            await _paymentTransactionRepository
                .CreateSSLPaymentRequestViewModelAsync(
                    memberId,
                    cycleId,
                    billId);

        if (paymentVM == null)
        {
            TempData["Error"] = "Payment info not found or already paid.";
            return RedirectToAction("Index", "MealBill");
        }

        var payload = new PaymentPayload
        {
            BillId = billId,
            CycleId = cycleId,
            MemberId = memberId,
            ExpireAt = DateTime.UtcNow.AddMinutes(15)
        };

        var token = PaymentTokenHelper.Generate(payload);
        var host = $"{Request.Scheme}://{Request.Host}";

        var request = new SSLPaymentRequest(
            Amount: paymentVM.Amount,
            TransactionId: Guid.NewGuid().ToString(),
            SuccessUrl: $"{host}/payment/success?token={Uri.EscapeDataString(token)}",
            FailUrl: $"{host}/payment/fail",
            CancelUrl: $"{host}/payment/cancel",
            CustomerName: paymentVM.CustomerName,
            CustomerEmail: paymentVM.CustomerEmail,
            CustomerPhone: paymentVM.CustomerPhone,
            ProductName: paymentVM.ProductName,
            value_a: billId.ToString(),
            value_b: cycleId.ToString()
        );

        var gatewayUrl = await _ssl.CreatePaymentAsync(request);
        return Redirect(gatewayUrl);
    }

    // ===========================
    // PAYMENT SUCCESS
    // ===========================
    public async Task<IActionResult> Success(string val_id, string token)
    {
        if (string.IsNullOrEmpty(val_id) || string.IsNullOrEmpty(token))
            return RedirectToAction(nameof(Fail));

        var payload = PaymentTokenHelper.Validate(token);
        if (payload == null)
            return RedirectToAction(nameof(Fail));

        long billId = payload.BillId;
        long cycleId = payload.CycleId;
        long memberId = payload.MemberId;

        try
        {
            var json = await _ssl.ValidateAsync(val_id);
            var paymentResult = ParsePaymentResult(json);

            if (paymentResult.Status != "Success")
                return RedirectToAction(nameof(Fail));

            // 🔹 Save Transaction
            var transaction = new PaymentTransaction
            {
                TransactionId = paymentResult.TransactionId,
                ValidationId = val_id,
                Amount = paymentResult.Amount,
                Currency = "BDT",
                Status = paymentResult.Status,
                TransactionDate = DateTime.Now,
                ValidatedOn = DateTime.Now,
                MemberId = memberId,
                MealBillId = billId,
                MealCycleId = cycleId
            };

            //await _paymentTransactionRepository.InsertAsync(transaction);

            // 🔹 Update Bill
            var bill = await _mealBillRepository.FindAsync(x => x.Id == billId);

            if (bill != null)
            {
                bill.TotalPaidAmount += transaction.Amount;
                bill.NetPayable = Math.Max(
                    bill.TotalPayable - bill.TotalPaidAmount, 0);

                bill.PaidStatus = bill.NetPayable == 0
                    ? "Paid"
                    : "Partial";

                await _mealBillRepository.UpdateAsync(bill);
            }

            // ================= EMAIL AFTER PAYMENT SUCCESS =================
            var user = await _userManager.FindByIdAsync(memberId.ToString());

            if (user != null && !string.IsNullOrWhiteSpace(user.Email))
            {
                var subject = "Payment Successful – Hostel Meal Management";

                var body = $@"
                    <h3>Hello {paymentResult.CustomerName}</h3>
                    <p>Your payment has been completed successfully.</p>
                    <p><b>Transaction ID:</b> {transaction.TransactionId}</p>
                    <p><b>Amount Paid:</b> {transaction.Amount} BDT</p>
                    <p><b>Date:</b> {DateTime.Now:dd MMM yyyy hh:mm tt}</p>
                    <br/>
                    <p>Thank you for your payment.</p>
                    <p>Hostel Meal Management System</p>
                ";

                await _emailService.SendAsync(user.Email, subject, body);
            }
            // ===============================================================

            return View(new PaymentSuccessViewModel
            {
                TransactionId = transaction.TransactionId,
                Amount = transaction.Amount,
                Status = transaction.Status,
                Message = "Payment completed successfully",
                PaymentDate = DateTime.Now,
                CustomerName = paymentResult.CustomerName ?? "Customer"
            });
        }
        catch
        {
            return RedirectToAction(nameof(Fail));
        }
    }

    // ===========================
    // FAIL / CANCEL
    // ===========================
    public IActionResult Fail() => View();
    public IActionResult Cancel() => View();

    // ===========================
    // JSON PARSER
    // ===========================
    private PaymentResult ParsePaymentResult(string json)
    {
        var data = JsonSerializer.Deserialize<SSLValidationResponse>(json);

        if (data == null)
            return new PaymentResult { Status = "Failed" };

        return new PaymentResult
        {
            TransactionId = data.tran_id,
            Amount = Convert.ToDecimal(data.amount),
            Status = data.status == "VALID" ? "Success" : "Failed",
            CustomerName = data.cus_name,
            MealBillId = long.TryParse(data.value_a, out var billId) ? billId : 0,
            MealCycleId = long.TryParse(data.value_b, out var cycleId) ? cycleId : 0
        };
    }
}
