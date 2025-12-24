using AutoMapper;
using HostelMealManagement.Application.Logging;
using HostelMealManagement.Application.Repositories;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HostelMealManagement.Web.Controllers;

[Authorize]
public class UtilityBillController(
    IUtilityBillRepository utilityBillRepository,
    IAppLogger<UtilityBillController> logger,
    IMapper mapper) : Controller
{
    private readonly IUtilityBillRepository _utilityBillRepository = utilityBillRepository;
    private readonly IAppLogger<UtilityBillController> _logger = logger;
    private readonly IMapper _mapper = mapper;

    [Route("utilitybill")]
    public async Task<IActionResult> Index()
    {
        try
        {
#if DEBUG
            _logger.LogInfo("Start Watch");
            var stopwatch = Stopwatch.StartNew();
#endif
            var utilityBills = await _utilityBillRepository.GetAllAsync();

#if DEBUG
            _logger.LogInfo($"GetAllAsync took {stopwatch.ElapsedMilliseconds}ms");
#endif
            _logger.LogInfo("Fetched UtilityBills");
            return View(_mapper.Map<List<UtilityBillVm>>(utilityBills));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while fetching UtilityBills", ex);
            return StatusCode(500, "An error occurred while fetching UtilityBills.");
        }
    }

    [HttpGet]
    [Route("utilitybill/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(long id = 0)
    {
        try
        {
            if (id > 0)
            {
                _logger.LogInfo($"Editing UtilityBill Id={id}");
                var utilityBill = await _utilityBillRepository.FindAsync(id);

                if (utilityBill == null)
                {
                    TempData["AlertMessage"] = $"UtilityBill with Id {id} not found.";
                    TempData["AlertType"] = "Error";
                    return RedirectToAction(nameof(Index));
                }

                return View(_mapper.Map<UtilityBillVm>(utilityBill));
            }

            return View(new UtilityBillVm());
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in CreateOrEdit for Id={id}", ex);
            return StatusCode(500, "An error occurred while opening the form.");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("utilitybill/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(UtilityBillVm utilityBillVm)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Please fix validation errors.";
            TempData["AlertType"] = "Warning";
            return View(utilityBillVm);
        }

        try
        {
            var utilityBillEntity = _mapper.Map<UtilityBill>(utilityBillVm);

            if (utilityBillVm.Id > 0)
            {
                _logger.LogInfo($"Updating UtilityBill Id={utilityBillVm.Id}");
                await _utilityBillRepository.UpdateAsync(utilityBillEntity);
                TempData["AlertMessage"] = "UtilityBill updated successfully!";
            }
            else
            {
                _logger.LogInfo("Creating new UtilityBill");
                await _utilityBillRepository.InsertAsync(utilityBillEntity);
                TempData["AlertMessage"] = "UtilityBill created successfully!";
            }

            TempData["AlertType"] = "Success";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while creating/updating UtilityBill", ex);
            TempData["AlertMessage"] = "An error occurred while saving the UtilityBill.";
            TempData["AlertType"] = "Error";
            return StatusCode(500);
        }
    }

    [HttpPost]
    [Route("utilitybill/delete/{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var utilityBill = await _utilityBillRepository.FindAsync(id);
            if (utilityBill == null)
            {
                TempData["AlertMessage"] = $"UtilityBill with Id {id} not found.";
                TempData["AlertType"] = "Error";
                return NotFound();
            }

            await _utilityBillRepository.DeleteAsync(utilityBill);

            TempData["AlertMessage"] = "UtilityBill deleted successfully!";
            TempData["AlertType"] = "Success";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while deleting UtilityBill Id={id}", ex);
            TempData["AlertMessage"] = "An error occurred while deleting the UtilityBill.";
            TempData["AlertType"] = "Error";
            return StatusCode(500);
        }
    }
}
