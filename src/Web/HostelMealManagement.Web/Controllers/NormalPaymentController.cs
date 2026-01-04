using AutoMapper;
using HostelMealManagement.Application.Enums;
using HostelMealManagement.Application.Logging;
using HostelMealManagement.Application.Repositories;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HostelMealManagement.Web.Controllers;

[Authorize]
public class NormalPaymentController : Controller
{
    private readonly INormalPaymentRepository _normalPaymentRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IMealCycleRepository _mealCycleRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<NormalPaymentController> _logger;
    private readonly IMealBillRepository _mealBillRepository;

    public NormalPaymentController(
        INormalPaymentRepository normalPaymentRepository,
        IMemberRepository memberRepository,
        IMealCycleRepository mealCycleRepository,
        IMapper mapper,
        IAppLogger<NormalPaymentController> logger,
        IMealBillRepository mealBillRepository)
    {
        _normalPaymentRepository = normalPaymentRepository;
        _memberRepository = memberRepository;
        _mealCycleRepository = mealCycleRepository;
        _mapper = mapper;
        _logger = logger;
        _mealBillRepository = mealBillRepository;
    }

    // ========================= INDEX =========================
    [HttpGet("/NormalPayment")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var payments = await _normalPaymentRepository.GetAllAsync();
            var paymentVmList = _mapper.Map<List<NormalPaymentVm>>(payments);
            return View(paymentVmList);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error fetching normal payments", ex);
            return StatusCode(500);
        }
    }

    // ========================= CREATE / EDIT (GET) =========================
    [HttpGet]
    [Route("NormalPayment/CreateOrEdit/{Id?}")]
    public async Task<IActionResult> CreateOrEdit(long Id = 0)
    {
        try
        {
            var vm = new NormalPaymentVm();

            ViewBag.MemberList = (await _memberRepository.GetAllAsync())
                .Where(x => !x.IsDelete)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();

            ViewBag.CycleList = (await _mealCycleRepository.GetAllAsync())
                .Where(x => !x.IsDelete)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();

            if (Id > 0)
            {
                var payment = await _normalPaymentRepository.FindAsync(Id);
                if (payment == null)
                {
                    TempData["AlertMessage"] = "Payment not found.";
                    TempData["AlertType"] = "Error";
                    return RedirectToAction(nameof(Index));
                }

                vm = _mapper.Map<NormalPaymentVm>(payment);
            }

            return View(vm);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error loading NormalPayment form", ex);
            return StatusCode(500);
        }
    }

    // ========================= CREATE / EDIT (POST) =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("NormalPayment/CreateOrEdit/{Id?}")]
    public async Task<IActionResult> CreateOrEdit(NormalPaymentVm vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.MemberList = (await _memberRepository.GetAllAsync())
                .Where(x => !x.IsDelete)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();

            ViewBag.CycleList = (await _mealCycleRepository.GetAllAsync())
                .Where(x => !x.IsDelete)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();

            return View(vm);
        }

        try
        {
            if (vm.PaymentAmount <= 0)
            {
                TempData["AlertMessage"] = "Payment amount is invalid.";
                TempData["AlertType"] = "Error";
                return RedirectToAction(nameof(Index));
            }

            decimal oldPaymentAmount = 0;
            var entity = _mapper.Map<NormalPayment>(vm);

            // ✅ FIX 1: SINGLE PREDICATE
            var bill = await _mealBillRepository.FindAsync(
                x => x.MemberId == vm.MemberId && x.MealCycleId == vm.CycleId);

            if (vm.Id > 0)
            {
                var payment = await _normalPaymentRepository.FindAsync(vm.Id);
                if (payment != null)
                {
                    // ✅ FIX 2: read OLD payment amount from DB
                    oldPaymentAmount = payment.PaymentAmount;
                }

                await _normalPaymentRepository.UpdateAsync(entity);
            }
            else
            {
                await _normalPaymentRepository.InsertAsync(entity);
            }

            if (bill is not null)
            {
                bill.TotalPaidAmount =
                    bill.TotalPaidAmount - oldPaymentAmount + entity.PaymentAmount;

                bill.NetPayable = Math.Max(
                    bill.TotalPayable - bill.TotalPaidAmount, 0);

                if (bill.NetPayable == 0)
                    bill.PaidStatus = "Paid";
                else if (bill.TotalPaidAmount > 0)
                    bill.PaidStatus = "Partial";
                else
                    bill.PaidStatus = "Unpaid";

                await _mealBillRepository.UpdateAsync(bill);
            }

            TempData["AlertMessage"] =
                vm.Id > 0 ? "Payment updated successfully!" : "Payment created successfully!";
            TempData["AlertType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error saving NormalPayment", ex);
            TempData["AlertMessage"] = "An error occurred while saving the payment.";
            TempData["AlertType"] = "Error";
            return StatusCode(500);
        }
    }

    // ========================= AUTO PAYMENT AMOUNT (AJAX) =========================
    [HttpGet]
    public async Task<IActionResult> GetCyclePayableAmount(long memberId, long cycleId)
    {
        var bill = await _mealBillRepository.FindAsync(
            x => x.MemberId == memberId && x.MealCycleId == cycleId);

        if (bill == null)
            return Json(new { amount = 0 });

        return Json(new { amount = bill.NetPayable });
    }
}
