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
public class MealAttendanceController(IMealAttendanceRepository attendanceRepository,
                                      IAppLogger<MealAttendanceController> logger,
                                      IMapper mapper) : Controller
{
    private readonly IMealAttendanceRepository _attendanceRepository = attendanceRepository;
    private readonly IAppLogger<MealAttendanceController> _logger = logger;
    private readonly IMapper _mapper = mapper;


    // --------------------------------------------------------
    // INDEX
    // --------------------------------------------------------
    [Route("mealattendance")]
    public async Task<IActionResult> Index()
    {
        try
        {
#if DEBUG
            _logger.LogInfo("Start Watch");
            var stopwatch = Stopwatch.StartNew();
#endif

            var attendances = await _attendanceRepository.GetAllAsync();

#if DEBUG
            _logger.LogInfo($"GetAllAsync took {stopwatch.ElapsedMilliseconds}ms");
#endif

            _logger.LogInfo("Fetched MealAttendances");

            return View(_mapper.Map<List<MealAttendanceVm>>(attendances));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while fetching Meal Attendances", ex);
            return StatusCode(500, "An error occurred while fetching Meal Attendance records.");
        }
    }


    // --------------------------------------------------------
    // GET: CREATE OR EDIT
    // --------------------------------------------------------
    [HttpGet]
    [Route("mealattendance/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(long id = 0)
    {
        try
        {
            if (id > 0)
            {
                _logger.LogInfo($"Editing MealAttendance Id={id}");

                var attendance = await _attendanceRepository.FindAsync(id);
                if (attendance == null)
                {
                    TempData["AlertMessage"] = $"MealAttendance with Id {id} not found.";
                    TempData["AlertType"] = "Error";
                    return RedirectToAction(nameof(Index));
                }

                return View(_mapper.Map<MealAttendanceVm>(attendance));
            }

            return View(new MealAttendanceVm());
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error opening CreateOrEdit for Id={id}", ex);
            return StatusCode(500, "An error occurred while opening the form.");
        }
    }


    // --------------------------------------------------------
    // POST: CREATE OR EDIT
    // --------------------------------------------------------
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("mealattendance/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(MealAttendanceVm attendanceVm)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Please fix validation errors.";
            TempData["AlertType"] = "Warning";
            return View(attendanceVm);
        }

        try
        {
            var attendanceEntity = _mapper.Map<MealAttendance>(attendanceVm);

            if (attendanceVm.Id > 0)
            {
                _logger.LogInfo($"Updating MealAttendance Id={attendanceVm.Id}");
                await _attendanceRepository.UpdateAsync(attendanceEntity);
                TempData["AlertMessage"] = "Meal Attendance updated successfully!";
            }
            else
            {
                _logger.LogInfo("Creating new MealAttendance");
                await _attendanceRepository.InsertAsync(attendanceEntity);
                TempData["AlertMessage"] = "Meal Attendance created successfully!";
            }

            TempData["AlertType"] = "Success";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while creating/updating Meal Attendance", ex);
            TempData["AlertMessage"] = "An error occurred while saving Meal Attendance.";
            TempData["AlertType"] = "Error";

            return StatusCode(500);
        }
    }


    // --------------------------------------------------------
    // DELETE
    // --------------------------------------------------------
    [HttpPost]
    [Route("mealattendance/delete/{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var attendance = await _attendanceRepository.FindAsync(id);

            if (attendance == null)
            {
                TempData["AlertMessage"] = $"MealAttendance with Id {id} not found.";
                TempData["AlertType"] = "Error";
                return NotFound();
            }

            await _attendanceRepository.DeleteAsync(attendance);

            TempData["AlertMessage"] = "Meal Attendance deleted successfully!";
            TempData["AlertType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting MealAttendance Id={id}", ex);
            TempData["AlertMessage"] = "An error occurred while deleting Meal Attendance.";
            TempData["AlertType"] = "Error";

            return StatusCode(500);
        }
    }
}
