using AutoMapper;
using HostelMealManagement.Application.Logging;
using HostelMealManagement.Application.Repositories;
using HostelMealManagement.Application.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace HostelMealManagement.Web.Controllers;

[Authorize]
public class ElectricBillController(
    IElectricBillRepository electricBillRepository,
    IMealCycleRepository mealCycleRepository,
    IAppLogger<ElectricBillController> logger,
    IMapper mapper
) : Controller
{
    private readonly IElectricBillRepository _electricBillRepository = electricBillRepository;
    private readonly IMealCycleRepository _mealCycleRepository = mealCycleRepository;
    private readonly IAppLogger<ElectricBillController> _logger = logger;
    private readonly IMapper _mapper = mapper;

    // =========================================================
    // INDEX
    // =========================================================
    [Route("electricbill")]
    public async Task<IActionResult> Index()
    {
        try
        {
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif

            var bills = await _electricBillRepository.GetAllAsync();

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

    // =========================================================
    // GET: CREATE / EDIT
    // =========================================================
    [HttpGet]
    [Route("electricbill/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(long id = 0)
    {
        try
        {
            await BindMealCycles(); // ✅ FIX

            if (id > 0)
            {
                var bill = await _electricBillRepository.GetByIdAsync(id);

                if (bill == null)
                {
                    TempData["AlertMessage"] = $"Electric Bill with Id {id} not found.";
                    TempData["AlertType"] = "Error";
                    return RedirectToAction(nameof(Index));
                }

                return View(bill);
            }

            return View(new ElectricBillVm());
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error opening CreateOrEdit for Id={id}", ex);
            return StatusCode(500, "An error occurred while opening the form.");
        }
    }

    // =========================================================
    // POST: CREATE / EDIT
    // =========================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("electricbill/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(ElectricBillVm vm)
    {
        await BindMealCycles(); // ✅ FIX (VERY IMPORTANT)

        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Please fix validation errors.";
            TempData["AlertType"] = "Warning";
            return View(vm);
        }

        try
        {
            // 🔢 AUTO CALCULATION
            vm.TotalUnit = vm.CurrentUnit - vm.PreviousUnit;
            vm.TotalAmount = vm.TotalUnit * vm.PerUnitRate;

            bool result = await _electricBillRepository.UpsertAsync(vm);

            if (!result)
            {
                TempData["AlertMessage"] = "Saving Electric Bill failed!";
                TempData["AlertType"] = "Error";
                return View(vm);
            }

            TempData["AlertMessage"] =
                vm.Id > 0
                ? "Electric Bill updated successfully!"
                : "Electric Bill created successfully!";

            TempData["AlertType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while saving Electric Bill", ex);
            TempData["AlertMessage"] = "An error occurred while saving Electric Bill.";
            TempData["AlertType"] = "Error";

            return StatusCode(500);
        }
    }

    // =========================================================
    // DELETE
    // =========================================================
    [HttpPost]
    [Route("electricbill/delete/{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var success = await _electricBillRepository.DeleteAsync(id);

            if (!success)
            {
                TempData["AlertMessage"] = $"Electric Bill with Id {id} not found!";
                TempData["AlertType"] = "Error";
                return NotFound();
            }

            TempData["AlertMessage"] = "Electric Bill deleted successfully!";
            TempData["AlertType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting Electric Bill Id={id}", ex);
            TempData["AlertMessage"] = "An error occurred while deleting Electric Bill.";
            TempData["AlertType"] = "Error";

            return StatusCode(500);
        }
    }

    // =========================================================
    // SHARED DROPDOWN BINDER (THE KEY FIX)
    // =========================================================
    private async Task BindMealCycles()
    {
        var cycles = await _mealCycleRepository.GetAllAsync();

        ViewBag.MealCycleId = cycles
            .Where(x => !x.IsDelete)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            })
            .ToList();
    }
}
