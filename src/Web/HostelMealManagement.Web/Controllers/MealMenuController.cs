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
public class MealMenuController(IMealMenuRepository mealMenuRepository, IAppLogger<MealMenuController> logger, IMapper mapper) : Controller
{
    private readonly IMealMenuRepository _mealMenuRepository = mealMenuRepository;
    private readonly IAppLogger<MealMenuController> _logger = logger;
    private readonly IMapper _mapper = mapper;

    [Route("mealmenu")]
    public async Task<IActionResult> Index()
    {
        try
        {
#if DEBUG
            _logger.LogInfo("Start Watch");
            var stopwatch = Stopwatch.StartNew();
#endif

            var menus = await _mealMenuRepository.GetAllAsync();

#if DEBUG
            _logger.LogInfo($"GetAllAsync took {stopwatch.ElapsedMilliseconds}ms");
#endif

            _logger.LogInfo("Fetched MealMenus");

            return View(_mapper.Map<List<MealMenuVm>>(menus));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while fetching MealMenus", ex);
            return StatusCode(500, "An error occurred while fetching MealMenus.");
        }
    }

    [HttpGet]
    [Route("mealmenu/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(long id = 0)
    {
        try
        {
            if (id > 0)
            {
                _logger.LogInfo($"Editing MealMenu Id={id}");
                var entity = await _mealMenuRepository.FindAsync(id);

                if (entity == null)
                {
                    TempData["AlertMessage"] = $"MealMenu with Id {id} not found.";
                    TempData["AlertType"] = "Error";
                    return RedirectToAction(nameof(Index));
                }

                return View(_mapper.Map<MealMenuVm>(entity));
            }

            return View(new MealMenuVm());
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in CreateOrEdit for Id={id}", ex);
            return StatusCode(500, "An error occurred while opening the form.");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("mealmenu/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(MealMenuVm menuVm)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Please fix validation errors.";
            TempData["AlertType"] = "Warning";
            return View(menuVm);
        }

        try
        {
            var entity = _mapper.Map<MealMenu>(menuVm);

            if (menuVm.Id > 0)
            {
                _logger.LogInfo($"Updating MealMenu Id={menuVm.Id}");
                await _mealMenuRepository.UpdateAsync(entity);

                TempData["AlertMessage"] = "MealMenu updated successfully!";
            }
            else
            {
                _logger.LogInfo("Creating new MealMenu");
                await _mealMenuRepository.InsertAsync(entity);

                TempData["AlertMessage"] = "MealMenu created successfully!";
            }

            TempData["AlertType"] = "Success";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while creating/updating MealMenu", ex);
            TempData["AlertMessage"] = "An error occurred while saving the MealMenu.";
            TempData["AlertType"] = "Error";
            return StatusCode(500);
        }
    }

    [HttpPost]
    [Route("mealmenu/delete/{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var entity = await _mealMenuRepository.FindAsync(id);
            if (entity == null)
            {
                TempData["AlertMessage"] = $"MealMenu with Id {id} not found.";
                TempData["AlertType"] = "Error";
                return NotFound();
            }

            await _mealMenuRepository.DeleteAsync(entity);

            TempData["AlertMessage"] = "MealMenu deleted successfully!";
            TempData["AlertType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while deleting MealMenu Id={id}", ex);
            TempData["AlertMessage"] = "An error occurred while deleting the MealMenu.";
            TempData["AlertType"] = "Error";
            return StatusCode(500);
        }
    }
}
