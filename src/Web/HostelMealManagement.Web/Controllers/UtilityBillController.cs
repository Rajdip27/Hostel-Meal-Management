using HostelMealManagement.Application.Logging;
using HostelMealManagement.Application.Repositories;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelMealManagement.Web.Controllers;

[Authorize]
public class UtilityBillController : Controller
{
    private readonly IUtilityBillRepository _utilityBillRepository;
    private readonly IAppLogger<UtilityBillController> _logger;

    public UtilityBillController(
        IUtilityBillRepository utilityBillRepository,
        IAppLogger<UtilityBillController> logger)
    {
        _utilityBillRepository = utilityBillRepository;
        _logger = logger;
    }

    // ================= INDEX =================
    [HttpGet]
    [Route("utility-bill")]
    public async Task<IActionResult> Index()
    {
        var bills = await _utilityBillRepository.GetAllAsync();
        return View(bills);
    }

    // ================= GET CREATE / EDIT =================
    [HttpGet]
    [Route("utility-bill/create-or-edit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(long? id)
    {
        if (id == null || id == 0)
        {
            return View(new UtilityBillVm
            {
                BillYear = DateTime.Now.Year,
                BillMonth = DateTime.Now.Month,
                BillDate = DateTimeOffset.Now
            });
        }

        var vm = await _utilityBillRepository.GetByIdAsync(id.Value);
        if (vm == null) return NotFound();

        return View(vm);
    }

    // ================= POST CREATE / EDIT =================
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("utility-bill/create-or-edit")]
    public async Task<IActionResult> CreateOrEdit(UtilityBillVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            // ELECTRIC
            if (vm.CurrentUnit.HasValue && vm.PerUnitRate.HasValue)
            {
                await _utilityBillRepository.UpsertAsync(new UtilityBillVm
                {
                    UtilityType = UtilityType.Electric,
                    BillYear = vm.BillYear,
                    BillMonth = vm.BillMonth,
                    BillDate = vm.BillDate,
                    CurrentUnit = vm.CurrentUnit,
                    PerUnitRate = vm.PerUnitRate
                });
            }

            // WATER
            if (vm.WaterAmount > 0)
            {
                await _utilityBillRepository.UpsertAsync(new UtilityBillVm
                {
                    UtilityType = UtilityType.Water,
                    BillYear = vm.BillYear,
                    BillMonth = vm.BillMonth,
                    BillDate = vm.BillDate,
                    TotalAmount = vm.WaterAmount
                });
            }

            // GAS
            if (vm.GasAmount > 0)
            {
                await _utilityBillRepository.UpsertAsync(new UtilityBillVm
                {
                    UtilityType = UtilityType.Gas,
                    BillYear = vm.BillYear,
                    BillMonth = vm.BillMonth,
                    BillDate = vm.BillDate,
                    TotalAmount = vm.GasAmount
                });
            }

            // SERVANT
            if (vm.ServantAmount > 0)
            {
                await _utilityBillRepository.UpsertAsync(new UtilityBillVm
                {
                    UtilityType = UtilityType.Servant,
                    BillYear = vm.BillYear,
                    BillMonth = vm.BillMonth,
                    BillDate = vm.BillDate,
                    TotalAmount = vm.ServantAmount
                });
            }

            TempData["AlertMessage"] = "Utility bills saved successfully.";
            TempData["AlertType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error saving utility bills", ex);
            TempData["AlertMessage"] = "An error occurred.";
            TempData["AlertType"] = "Error";
            return View(vm);
        }
    }

    // ================= DELETE =================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        await _utilityBillRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
