using AutoMapper;
using HostelMealManagement.Application.Helpers;
using HostelMealManagement.Application.Logging;
using HostelMealManagement.Application.Repositories;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HostelMealManagement.Web.Controllers;

[Authorize(Roles = Permissions.Admin)]
public class MealCycleController(IMealCycleRepository mealCycleRepository, IAppLogger<MealCycleController> logger, IMapper mapper) : Controller
{
    private readonly IMealCycleRepository _mealCycleRepository = mealCycleRepository;
    private readonly IAppLogger<MealCycleController> _logger = logger;
    private readonly IMapper _mapper = mapper;

    public async Task<IActionResult> Index()
    {
        try
        {
            #if DEBUG
            _logger.LogInfo("Start Watch");
            var stopwatch = Stopwatch.StartNew();
            #endif
            var mealCycles = await _mealCycleRepository.GetAllAsync();
            #if DEBUG
            _logger.LogInfo($"GetAllAsync took {stopwatch.ElapsedMilliseconds}ms");
            #endif
            _logger.LogInfo("Fetched MealCycles");
            return View(mealCycles);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while fetching MealCycles", ex);
            return StatusCode(500, "An error occurred while fetching MealCycles.");
        }
    }

    [HttpGet]
    [Route("mealcycle/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(long id = 0)
    {
        try
        {
            if (id > 0)
            {
                _logger.LogInfo($"Editing MealCycle Id={id}");
                var mealCycleVm = await _mealCycleRepository.FindAsync(id);

                if (mealCycleVm == null)
                {
                    TempData["AlertMessage"] = $"MealCycle with Id {id} not found.";
                    TempData["AlertType"] = "Error";
                    return RedirectToAction(nameof(Index));
                }

                return View(mealCycleVm);
            }

            return View(new MealCycleVm());
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in CreateOrEdit for Id={id}", ex);
            return StatusCode(500, "An error occurred while opening the form.");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("mealcycle/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(MealCycleVm mealCycleVm)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Please fix validation errors.";
            TempData["AlertType"] = "Warning";
            return View(mealCycleVm);
        }

        try
        {
            var mealCycleEntity = _mapper.Map<MealCycle>(mealCycleVm);

            if (mealCycleVm.Id > 0)
            {
                _logger.LogInfo($"Updating MealCycle Id={mealCycleVm.Id}");
                await _mealCycleRepository.UpdateAsync(mealCycleEntity);
                TempData["AlertMessage"] = "MealCycle updated successfully!";
            }
            else
            {
                _logger.LogInfo("Creating new MealCycle");
                await _mealCycleRepository.InsertAsync(mealCycleEntity);
                TempData["AlertMessage"] = "MealCycle created successfully!";
            }

            TempData["AlertType"] = "Success";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while creating/updating MealCycle", ex);
            TempData["AlertMessage"] = "An error occurred while saving the MealCycle.";
            TempData["AlertType"] = "Error";
            return StatusCode(500);
        }
    }

    [HttpPost]
    [Route("mealcycle/delete/{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var mealCycleVm = await _mealCycleRepository.FindAsync(id);
            if (mealCycleVm == null)
            {
                TempData["AlertMessage"] = $"MealCycle with Id {id} not found.";
                TempData["AlertType"] = "Error";
                return NotFound();
            }

            await _mealCycleRepository.DeleteAsync(mealCycleVm);

            TempData["AlertMessage"] = "MealCycle deleted successfully!";
            TempData["AlertType"] = "Success";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while deleting MealCycle Id={id}", ex);
            TempData["AlertMessage"] = "An error occurred while deleting the MealCycle.";
            TempData["AlertType"] = "Error";
            return StatusCode(500);
        }
    }
}
