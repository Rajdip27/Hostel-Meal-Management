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
public class MealMenuController(
    IMealMenuRepository mealMenuRepository,
    IAppLogger<MealMenuController> logger,
    IMapper mapper) : Controller
{
    private readonly IMealMenuRepository _mealMenuRepository = mealMenuRepository;
    private readonly IAppLogger<MealMenuController> _logger = logger;
    private readonly IMapper _mapper = mapper;

    // ========================= INDEX =========================
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
            return View(_mapper.Map<List<MealMenuVm>>(menus));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while fetching MealMenus", ex);
            return StatusCode(500);
        }
    }

    // ========================= GET CREATE / EDIT =========================
    [HttpGet]
    [Route("mealmenu/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(long id = 0)
    {
        try
        {
            if (id > 0)
            {
                var entity = await _mealMenuRepository.FindAsync(id);
                if (entity == null)
                {
                    TempData["AlertMessage"] = "Meal menu not found.";
                    TempData["AlertType"] = "Error";
                    return RedirectToAction(nameof(Index));
                }

                // 🔒 Block edit if inactive
                if (!entity.IsActive)
                {
                    TempData["AlertMessage"] = "This meal menu is locked. Please unlock it first.";
                    TempData["AlertType"] = "Warning";
                    return RedirectToAction(nameof(Index));
                }

                return View(_mapper.Map<MealMenuVm>(entity));
            }

            return View(new MealMenuVm());
        }
        catch (Exception ex)
        {
            _logger.LogError("Error opening CreateOrEdit", ex);
            return StatusCode(500);
        }
    }

    // ========================= POST CREATE / EDIT =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("mealmenu/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(MealMenuVm menuVm)
    {
        if (!ModelState.IsValid)
            return View(menuVm);

        try
        {
            if (menuVm.Id > 0)
            {
                var existing = await _mealMenuRepository.FindAsync(menuVm.Id);
                if (existing == null)
                    return NotFound();

                // 🔒 Block update if inactive
                if (!existing.IsActive)
                {
                    TempData["AlertMessage"] = "Inactive meal menu cannot be updated.";
                    TempData["AlertType"] = "Error";
                    return RedirectToAction(nameof(Index));
                }

                // 🔒 Preserve fixed MealName
                menuVm.MealName = existing.MealName;

                var entity = _mapper.Map(menuVm, existing);
                await _mealMenuRepository.UpdateAsync(entity);

                TempData["AlertMessage"] = "Meal menu updated successfully!";
            }
            else
            {
                var entity = _mapper.Map<MealMenu>(menuVm);
                await _mealMenuRepository.InsertAsync(entity);

                TempData["AlertMessage"] = "Meal menu created successfully!";
            }

            TempData["AlertType"] = "Success";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error saving MealMenu", ex);
            TempData["AlertMessage"] = "An error occurred while saving.";
            TempData["AlertType"] = "Error";
            return StatusCode(500);
        }
    }

    // ========================= DELETE =========================
    [HttpPost]
    [Route("mealmenu/delete/{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var entity = await _mealMenuRepository.FindAsync(id);
            if (entity == null)
                return NotFound();

            // 🔒 Block delete if inactive
            if (!entity.IsActive)
            {
                TempData["AlertMessage"] = "Inactive meal menu cannot be deleted.";
                TempData["AlertType"] = "Error";
                return RedirectToAction(nameof(Index));
            }

            await _mealMenuRepository.DeleteAsync(entity);

            TempData["AlertMessage"] = "Meal menu deleted successfully!";
            TempData["AlertType"] = "Success";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error deleting MealMenu", ex);
            TempData["AlertMessage"] = "Error deleting meal menu.";
            TempData["AlertType"] = "Error";
            return StatusCode(500);
        }
    }

    // ========================= UNLOCK =========================
    [HttpPost]
    [Route("mealmenu/unlock/{id}")]
    public async Task<IActionResult> Unlock(long id)
    {
        try
        {
            var entity = await _mealMenuRepository.FindAsync(id);
            if (entity == null)
            {
                TempData["AlertMessage"] = "Meal menu not found.";
                TempData["AlertType"] = "Error";
                return RedirectToAction(nameof(Index));
            }

            entity.IsActive = true;
            await _mealMenuRepository.UpdateAsync(entity);

            TempData["AlertMessage"] = "Meal menu unlocked successfully!";
            TempData["AlertType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error unlocking MealMenu", ex);
            TempData["AlertMessage"] = "Error unlocking meal menu.";
            TempData["AlertType"] = "Error";
            return StatusCode(500);
        }
    }
}
