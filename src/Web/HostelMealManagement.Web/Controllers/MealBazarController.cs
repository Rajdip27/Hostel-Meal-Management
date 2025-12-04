using HostelMealManagement.Application.Helpers;
using HostelMealManagement.Application.Logging;
using HostelMealManagement.Application.Repositories;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HostelMealManagement.Web.Controllers;

[Authorize]
public class MealBazarController(IMealBazarRepository mealBazarRepository,
                                IAppLogger<MealBazarController> logger) : Controller
{
    private readonly IMealBazarRepository _mealBazarRepository = mealBazarRepository;
    private readonly IAppLogger<MealBazarController> _logger = logger;

    // ===================== INDEX ============================
    [Route("mealbazar")]
    public async Task<IActionResult> Index()
    {
        try
        {
#if DEBUG
            _logger.LogInfo("Start Watch");
            var stopwatch = Stopwatch.StartNew();
#endif
            var mealBazars = await _mealBazarRepository.GetAllAsync();

#if DEBUG
            _logger.LogInfo($"GetAllAsync took {stopwatch.ElapsedMilliseconds}ms");
#endif

            _logger.LogInfo("Fetched MealBazars");

    
           

            return View(mealBazars);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while fetching MealBazars", ex);
            return StatusCode(500, "An error occurred while fetching MealBazars.");
        }
    }

    // ===================== CREATE OR EDIT (GET) ============================
    [HttpGet]
    [Route("mealbazar/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(long id = 0)
    {
        try
        {
            if (id > 0)
            {
                _logger.LogInfo($"Editing MealBazar Id={id}");
                var mealBazar = await _mealBazarRepository.GetByIdAsync(id);

                if (mealBazar == null)
                {
                    TempData["AlertMessage"] = $"MealBazar with Id {id} not found.";
                    TempData["AlertType"] = "Error";
                    return RedirectToAction(nameof(Index));
                }
                return View(mealBazar);
            }

            return View(new MealBazarVm());
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in CreateOrEdit for Id={id}", ex);
            return StatusCode(500, "An error occurred while opening the form.");
        }
    }

    // ===================== CREATE OR EDIT (POST) ============================
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("mealbazar/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(MealBazarVm mealBazarVm)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Please fix validation errors.";
            TempData["AlertType"] = "Warning";
            return View(mealBazarVm);
        }

        try
        {

            if (mealBazarVm.Id > 0)
            {
                _logger.LogInfo($"Updating MealBazar Id={mealBazarVm.Id}");
                await _mealBazarRepository.UpsertAsync(mealBazarVm);

                TempData["AlertMessage"] = "MealBazar updated successfully!";
            }
            else
            {
                _logger.LogInfo("Creating new MealBazar");
                await _mealBazarRepository.UpsertAsync(mealBazarVm);

                TempData["AlertMessage"] = "MealBazar created successfully!";
            }

            TempData["AlertType"] = "Success";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while creating/updating MealBazar", ex);
            TempData["AlertMessage"] = "An error occurred while saving the MealBazar.";
            TempData["AlertType"] = "Error";
            return StatusCode(500);
        }
    }

    // ===================== DELETE ============================
    [HttpPost]
    [Route("mealbazar/delete/{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var mealBazar = await _mealBazarRepository.GetByIdAsync(id);

            if (mealBazar == null)
            {
                TempData["AlertMessage"] = $"MealBazar with Id {id} not found.";
                TempData["AlertType"] = "Error";
                return NotFound();
            }

            await _mealBazarRepository.DeleteAsync(id);

            TempData["AlertMessage"] = "MealBazar deleted successfully!";
            TempData["AlertType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while deleting MealBazar Id={id}", ex);
            TempData["AlertMessage"] = "An error occurred while deleting the MealBazar.";
            TempData["AlertType"] = "Error";

            return StatusCode(500);
        }
    }
}
