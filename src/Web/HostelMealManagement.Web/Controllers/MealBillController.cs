using HostelMealManagement.Application.CommonModel;
using HostelMealManagement.Application.Logging;
using HostelMealManagement.Application.Repositories;
using HostelMealManagement.Infrastructure.Helper.Acls;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HostelMealManagement.Web.Controllers;

public class MealBillController : Controller
{
    private readonly IMemberRepository _memberRepository;
    private readonly IMealCycleRepository _mealCycleRepository;
    private readonly  IMealBillRepository _mealAttendanceRepository;
    private ISignInHelper SignInHelper;
    private readonly IAppLogger<MealAttendanceController> _logger;

    public MealBillController(
        IMemberRepository memberRepository,
        IMealCycleRepository mealCycleRepository,
        IAppLogger<MealAttendanceController> logger,
        IMealBillRepository mealAttendanceRepository,
        ISignInHelper signInHelper)
    {
        _memberRepository = memberRepository;
        _mealCycleRepository = mealCycleRepository;
        _logger = logger;
        _mealAttendanceRepository = mealAttendanceRepository;
        SignInHelper = signInHelper;
    }


    [HttpGet]
    [Route("meala-bill")]
    public IActionResult MealAttendanceProcess()
    {
        ViewBag.Members = _memberRepository.GetMemberList();
        ViewBag.MealCycle = _mealCycleRepository.GetMealCycleList();
        return View();
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Process(FilterViewModel filter)
    {
        try
        {
            bool result = await _mealAttendanceRepository.GenerateMealBillAsync(filter.MealCycleId, SignInHelper.UserId ?? 0);

            if (result)
            {
                _logger.LogInfo($"Meal bill generated successfully for MealCycleId={filter.MealCycleId}");
                TempData["AlertMessage"] = "Meal bill generated successfully!";
                TempData["AlertType"] = "Success";
            }
            else
            {
                _logger.LogWarning($"Meal bill generation failed for MealCycleId={filter.MealCycleId}");
                TempData["AlertMessage"] = "Meal bill generation failed.";
                TempData["AlertType"] = "Warning";
            }

            return RedirectToAction(nameof(MealAttendanceProcess));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while generating meal bill for MealCycleId={filter.MealCycleId}", ex);
            TempData["AlertMessage"] = "An error occurred while generating the meal bill.";
            TempData["AlertType"] = "Error";
            return StatusCode(500);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(FilterViewModel model)
    {
        try
        {
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif

            var bills = await _mealAttendanceRepository.GetMealBillsWithMemberAsync(model.MealCycleId);

#if DEBUG
            _logger.LogInfo($"ElectricBill GetAllAsync took {sw.ElapsedMilliseconds}ms");
#endif

            return View(bills);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while fetching Electric Bills", ex);
            return StatusCode(500, "An error occurred while fetching Electric Bills.");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult PDF(FilterViewModel model)
    {
        TempData["AlertMessage"] = "PDF button clicked!";
        TempData["AlertType"] = "success";
        return RedirectToAction(nameof(MealAttendanceProcess));
    }
}
