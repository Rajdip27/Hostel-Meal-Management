using AutoMapper;
using HostelMealManagement.Application.Logging;
using HostelMealManagement.Application.Repositories;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.Helper.Acls;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static HostelMealManagement.Core.Entities.Auth.IdentityModel;

namespace HostelMealManagement.Web.Controllers;

[Authorize]
public class MealAttendanceController(
    IMealAttendanceRepository attendanceRepository,
    IAppLogger<MealAttendanceController> logger,
    IMapper mapper,
    ISignInHelper signInHelper,
    IMemberRepository memberRepository,
    UserManager<User> _userManager
) : Controller
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
    // GET: CREATE or EDIT
    // --------------------------------------------------------
    [HttpGet]
    [Route("mealattendance/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(long id = 0)
    {
        try
        {
            var roles = signInHelper.Roles;
            var userId = signInHelper.UserId;

            if (roles.Contains("Member") && userId.HasValue)
            {
                var user = await _userManager.FindByIdAsync(userId.Value.ToString());

                ViewBag.MemberId = memberRepository
                    .GetMemberList()
                    .Where(x => x.Value == user.MemberId.ToString())
                    .ToList();
            }
            else
            {
                ViewBag.MemberId = memberRepository.GetMemberList();
            }

            if (id > 0)
            {
                var attendance = await _attendanceRepository.GetByIdAsync(id);

                if (attendance == null)
                {
                    TempData["AlertMessage"] = $"MealAttendance with Id {id} not found.";
                    TempData["AlertType"] = "Error";
                    return RedirectToAction(nameof(Index));
                }

                return View(attendance);
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
    // POST: CREATE or EDIT
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
            bool result = await _attendanceRepository.UpsertAsync(attendanceVm);

            if (!result)
            {
                TempData["AlertMessage"] = "Saving Meal Attendance failed!";
                TempData["AlertType"] = "Error";
                return View(attendanceVm);
            }

            TempData["AlertMessage"] =
                attendanceVm.Id > 0
                ? "Meal Attendance updated successfully!"
                : "Meal Attendance created successfully!";

            TempData["AlertType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while saving Meal Attendance", ex);
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
            var success = await _attendanceRepository.DeleteAsync(id);

            if (!success)
            {
                TempData["AlertMessage"] = $"MealAttendance with Id {id} not found!";
                TempData["AlertType"] = "Error";
                return NotFound();
            }

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